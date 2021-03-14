using System.IO;
using System.Collections.Generic;

using RenderWareLib;
using RenderWareLib.Mathematics;
using RenderWareLib.SectionsData;

namespace GtaLib.DFF
{
    public class DFFLoader
    {
        private struct IndexedDFFFrame
        {
            public DFFFrame Frame;
            public int ParentIdx;
            public int BoneID;
        }

        public bool GotoClumpSection(BinaryReader br, out RWSectionHeader outHeader)
        {
            while (RWSectionHeader.ReadSectionHeader(br, out outHeader) && outHeader.Id != RWSectionId.RW_SECTION_CLUMP)
            {
                br.BaseStream.Position += outHeader.Size;
            }
            if (outHeader.Id == RWSectionId.RW_SECTION_CLUMP)
            {
                return true;
            }
            return false;
        }

        public RWSection LoadClumpSection(BinaryReader br)
        {
            // RWSectionHeader header;
            GtaLib.Experimental.ForceReader.ForceDFFReader fdr = new Experimental.ForceReader.ForceDFFReader(br);
            RWSection clump = fdr.Read();
            return clump;
            /*
            if (GotoClumpSection(br, out header))
            {
                return RWSection.ReadSectionBody(br, header);
            }
            return null;
            */
        }

        public DFFMesh LoadMesh(BinaryReader br)
        {
            RWSection sect = LoadClumpSection(br);
            if (sect == null)
            {
                return null;
            }
            DFFMesh mesh = LoadMesh(sect);
            return mesh;
        }

        public DFFMesh LoadMesh(RWSection clump)
        {
            DFFMesh mesh = new DFFMesh();
            // LOAD FRAMES
            RWSection frameList = clump.FindChild(RWSectionId.RW_SECTION_FRAMELIST);
            RWSection frameListStruct = frameList.FindChild(RWSectionId.RW_SECTION_STRUCT);
            RWFrameListData frameListStructData = frameListStruct.GetParsedData() as RWFrameListData;
            uint numFrames = frameListStructData.FramesCount;
            // In DFF, the frames are stored and indexed as a flat data structure, so at first we keep them flat.
            IndexedDFFFrame[] indexedFrames = new IndexedDFFFrame[numFrames];

            DFFFrame[] boneFrames = null;

            Dictionary<int, DFFBone> bonesByIndex = new Dictionary<int, DFFBone>();
            Dictionary<int, DFFBone> bonesByNum = new Dictionary<int, DFFBone>();

            for (int i = 0; i < numFrames; i++)
            {
                indexedFrames[i] = new IndexedDFFFrame();
                indexedFrames[i].BoneID = -1;
                indexedFrames[i].ParentIdx = (int)frameListStructData.Items[i].ParentFrame;
                RWMatrix4 mm = frameListStructData.Items[i].TransformMatrix;
                DFFFrame frame = new DFFFrame(mm);
                frame.Flags = frameListStructData.Items[i].Flags;
                indexedFrames[i].Frame = frame;
            }
            // Now we read the frame names.
            RWSection[] frameExtensions = frameList.FindChildCollection(RWSectionId.RW_SECTION_EXTENSION);
            for (int i = 0; i < frameExtensions.Length; i += 1)
            {
                if (i >= numFrames)
                {
                    throw new DFFException("Too many frame list extensions");
                }
                RWSection frameListExt = frameExtensions[i];
                RWSection frameSect = frameListExt.FindChild(RWSectionId.RW_SECTION_FRAME);
                if (frameSect != null)
                {
                    RWFrameData frmData = frameSect.GetParsedData() as RWFrameData;
                    indexedFrames[i].Frame.Name = frmData.FrameName.Trim();
                }
                RWSection hAnimPlg = frameListExt.FindChild(RWSectionId.RW_SECTION_HANIM_PLG);
                if (hAnimPlg != null)
                {
                    // We have a hierarchical animation plugin (HANIM_PLG) section. Such a section can exist for each frame, but
                    // does not have to. If it is present for a frame, this means that the frame acts as a bone for an animation,
                    // and the frame gets assigned a bone ID. HANIM_PLG also doubles as a section that describes the types and
                    // properties of bones when the boneCount property is not zero. This code currently assumes that all this
                    // additional bone information is contained completely in a single HANIM_PLG section, and not spread across
                    // multiple (which seems to be the case in all of the original DFF meshes).
                    RWHAnimPLGData adata = hAnimPlg.GetParsedData() as RWHAnimPLGData;
                    uint boneID = adata.BoneID;
                    uint boneCount = adata.BoneCount;
                    indexedFrames[i].BoneID = (int)boneID;
                    if (boneCount != 0)
                    {
                        mesh.BoneCount = (int)boneCount;

                        boneFrames = new DFFFrame[boneCount];
                        // We assume that all bone detail data is contained in a single section. If we have multiple sections
                        // containing bone detail, we keep the data of the more recent section.
                        bonesByNum.Clear();
                        bonesByIndex.Clear();

                        for (int j = 0; j < boneCount; j++)
                        {
                            // NOTE: The 'type' value might be the node topology flags. I don't know this for sure
                            // and we don't seem to need this either way, but it might be like this:
                            // 0 = NONE
                            // 1 = POP
                            // 2 = PUSH
                            // 3 = PUSH/POP
                            DFFBone bone = new DFFBone();
                            bone.Index = adata.BoneInformations[j].Index;
                            bone.Number = adata.BoneInformations[j].BoneId;
                            bone.Type = (DFFBoneType)adata.BoneInformations[j].Flags;

                            bonesByIndex.Add((int)bone.Index, bone);
                            bonesByNum.Add((int)bone.Number, bone);
                        }
                    }
                }
            }
            // Associate frames with bones
            if (bonesByIndex.Count != 0)
            {
                uint boneIdx = 0;
                for (int i = 0; i < numFrames; i++)
                {
                    if (indexedFrames[i].BoneID != -1)
                    {
                        indexedFrames[i].Frame.Bone = bonesByIndex[indexedFrames[i].BoneID];
                        boneFrames[boneIdx++] = indexedFrames[i].Frame;
                    }
                }
            }
            // And now we will actually build the frame hierarchy.
            // We still keep the flat structure (indexedFrames), because we will need this later when we read the
            // ATOMIC sections, which link frames and geometries.
            DFFFrame rootFrame = mesh.RootFrame;
            for (int i = 0; i < numFrames; i += 1)
            {
                IndexedDFFFrame indexedFrame = indexedFrames[i];
                if (indexedFrame.ParentIdx != -1)
                {
                    indexedFrames[indexedFrame.ParentIdx].Frame.AddChild(indexedFrame.Frame);
                }
                else
                {
                    rootFrame.AddChild(indexedFrame.Frame);
                }
            }
            //
            // Load the Geometries
            //
            RWSection geomList = clump.FindChild(RWSectionId.RW_SECTION_GEOMETRYLIST);
            RWSection geomListStruct = geomList.FindChild(RWSectionId.RW_SECTION_STRUCT);
            uint numGeoms = (geomListStruct.GetParsedData() as RWGeometryListData).GeometryCount;
            RWSection[] geomSections = geomList.FindChildCollection(RWSectionId.RW_SECTION_GEOMETRY);
            for (int i = 0; i < geomSections.Length; i += 1)
            {
                RWSection geomSect = geomSections[i];
                //
                // ********** LOAD THE GEOMETRY STRUCT **********
                //
                // This is most notably the vertex data.
                RWSection geomStruct = geomSect.FindChild(RWSectionId.RW_SECTION_STRUCT);
                RWGeometryData geomData = geomStruct.GetParsedData() as RWGeometryData;
                DFFGeometry geom = new DFFGeometry()
                {
                    Vertices = geomData.Vertices,
                    Normals = geomData.Normals,
                    UVSets = new DFFUvSet[geomData.TexCoords.Length],
                    VertexColors = geomData.VertexColors,
                    Flags = geomData.Flags,
                    Bounds = geomData.Bounds,
                    AmbientLight = geomData.AmbientColor,
                    DiffuseLight = geomData.DiffuseColor,
                    SpecularLight = geomData.SpecularColor,
                };
                for (int j = 0; j < geomData.TexCoords.Length; j += 1)
                {
                    geom.UVSets[j] = new DFFUvSet();
                    geom.UVSets[j].TexCoords = geomData.TexCoords[j];
                }
                // ********** LOAD THE MATERIALS **********
                RWSection materialList = geomSect.FindChild(RWSectionId.RW_SECTION_MATERIALLIST);
                RWSection materialListStruct = materialList.FindChild(RWSectionId.RW_SECTION_STRUCT);

                RWMaterialListData materialListStructData = materialListStruct.GetParsedData() as RWMaterialListData;

                RWSection[] materials = materialList.FindChildCollection(RWSectionId.RW_SECTION_MATERIAL);

                for (int j = 0; j < materials.Length; j += 1)
                {
                    RWSection matSect = materials[j];
                    RWSection matStruct = matSect.FindChild(RWSectionId.RW_SECTION_STRUCT);
                    RWMaterialData matStructData = matStruct.GetParsedData() as RWMaterialData;
                    DFFMaterial mat = new DFFMaterial();
                    mat.Color = matStructData.Color;
                    int texCount = matStructData.TextureCount;

                    // Load the textures
                    RWSection[] textures = matSect.FindChildCollection(RWSectionId.RW_SECTION_TEXTURE);
                    for (int k = 0; k < textures.Length; k += 1)
                    {
                        RWSection texSect = textures[k];
                        RWSection texStruct = texSect.FindChild(RWSectionId.RW_SECTION_STRUCT);

                        ushort filterFlags = (texStruct.GetParsedData() as RWTextureData).FilterFlags;

                        RWSection[] names = texSect.FindChildCollection(RWSectionId.RW_SECTION_STRING);
                        string diffuseName = "";
                        string alphaName = "";
                        if (names.Length > 0)
                        {
                            diffuseName = (names[0].GetParsedData() as RWStringData).Text;
                        }
                        if (names.Length > 1)
                        {
                            alphaName = (names[1].GetParsedData() as RWStringData).Text;
                        }
                        DFFTexture tex = new DFFTexture()
                        {
                            DiffuseName = diffuseName,
                            AlphaName = alphaName,
                            FilterModeFlags = filterFlags,
                        };
                        mat.AddTexture(tex);
                    }

                    geom.AddMaterial(mat);
                }

                // ********** Load the material split data **********
                RWSection geomExt = geomSect.FindChild(RWSectionId.RW_SECTION_EXTENSION);
                RWSection materialSplit = geomExt.FindChild(RWSectionId.RW_SECTION_MATERIALSPLIT);
                RWMaterialSplitData msData = materialSplit.GetParsedData() as RWMaterialSplitData;

                for (int j = 0; j < msData.Meshes.Length; j += 1)
                {
                    DFFGeometryPart part = new DFFGeometryPart();
                    part.Indices = msData.Meshes[j].Indices;
                    geom.AddPart(part);
                    part.SetMaterial(geom.GetMaterial((int)msData.Meshes[j].MaterialIndex));
                }

                // TODO: Load the vertex skinning data

                mesh.AddGeometry(geom);
            }

            // **********************************************************************
            // *				LOAD THE GEOMETRY <-> FRAME LINKS					*
            // **********************************************************************

            RWSection[] atomics = clump.FindChildCollection(RWSectionId.RW_SECTION_ATOMIC);
            for (int i = 0; i < atomics.Length; i += 1)
            {
                RWSection atomic = atomics[i];
                RWSection atomicStruct = atomic.FindChild(RWSectionId.RW_SECTION_STRUCT);
                RWAtomicData asData = atomicStruct.GetParsedData() as RWAtomicData;
                mesh.GetGeometry((int)asData.GeometryIndex).AssociatedFrame = indexedFrames[asData.FrameIndex].Frame;
            }

            // TODO: load integrated COL

            return mesh;
        }
    }
}
