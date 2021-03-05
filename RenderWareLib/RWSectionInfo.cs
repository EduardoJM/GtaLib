using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderWareLib
{
	public struct RWSectionInfo
	{
		public RWSectionId Id { get; set; }
		public string Name { get; set; }
		public bool Container { get; set; }

		private static RWSectionInfo _c(RWSectionId _id, string _name, bool _container)
		{
			return new RWSectionInfo()
			{
				Id = _id,
				Name = _name,
				Container = _container
			};
		}

		public static RWSectionInfo[] RWKnownSections = new RWSectionInfo[] {
			_c(RWSectionId.RW_SECTION_STRUCT, "STRUCT", false),
			_c(RWSectionId.RW_SECTION_STRING, "STRING", false),
			_c(RWSectionId.RW_SECTION_EXTENSION, "EXTENSION", true),
			_c(RWSectionId.RW_SECTION_CAMERA, "CAMERA", false),
			_c(RWSectionId.RW_SECTION_TEXTURE, "TEXTURE", true),
			_c(RWSectionId.RW_SECTION_MATERIAL, "MATERIAL", true),
			_c(RWSectionId.RW_SECTION_MATERIALLIST, "MATERIALLIST", true),
			_c(RWSectionId.RW_SECTION_FRAMELIST, "FRAMELIST", true),
			_c(RWSectionId.RW_SECTION_GEOMETRY, "GEOMETRY", true),
			_c(RWSectionId.RW_SECTION_CLUMP, "CLUMP", true),
			_c(RWSectionId.RW_SECTION_ATOMIC, "ATOMIC", true),
			_c(RWSectionId.RW_SECTION_GEOMETRYLIST, "GEOMETRYLIST", true),
			_c(RWSectionId.RW_SECTION_MATERIALSPLIT, "MATERIALSPLIT", false),
			_c(RWSectionId.RW_SECTION_FRAME, "FRAME", false),
			_c(RWSectionId.RW_SECTION_HANIM_PLG, "HANIM_PLG", false),
			_c(RWSectionId.RW_SECTION_SKY_MIPMAP_VAL, "SKY_MIPMAP_VAL", false),
			_c(RWSectionId.RW_SECTION_MATERIAL_EFFECTS_PLG, "MATERIAL_EFFECTS_PLG", true),
			_c(RWSectionId.RW_SECTION_SPECULAR_MATERIAL, "SPECULAR_MATERIAL", false),
			_c(RWSectionId.RW_SECTION_NIGHT_VERTEX_COLORS, "NIGHT_VERTEX_COLORS", false),
			_c(RWSectionId.RW_SECTION_REFLECTION_MATERIAL, "REFLECTION_MATERIAL", false),
			_c(RWSectionId.RW_SECTION_2DFX, "2DFX", false),
			_c(RWSectionId.RW_SECTION_MESH_EXTENSION, "MESH_EXTENSION", false),
			_c(RWSectionId.RW_SECTION_RIGHT_TO_RENDER, "RIGHT_TO_RENDER", false),
			_c(RWSectionId.RW_SECTION_COLLISION_MODEL, "COLLISION_MODEL", false),
			_c(RWSectionId.RW_SECTION_TEXTUREDICTIONARY, "TEXTUREDICTIONARY", true),
			_c(RWSectionId.RW_SECTION_TEXTURENATIVE, "TEXTURENATIVE", true),
			_c(RWSectionId.RW_SECTION_UV_ANIM_DICTIONARY, "UV_ANIM_DICTIONARY", true),
			_c(RWSectionId.RW_SECTION_UV_ANIM_PLG, "UV_ANIM_PLG", true),
			_c(RWSectionId.RW_SECTION_MORPH_PLG, "MORPH_PLG", false),
			_c(RWSectionId.RW_SECTION_PIPELINE_SET, "PIPELINE_SET", false)
		};

		public static bool RWGetSectionInfo(RWSectionId _id, out RWSectionInfo info)
		{
			info = _c(RWSectionId.RW_SECTION_INVALID, "INVALID", false);
			for (int i = 0; i < RWKnownSections.Length; i += 1)
            {
				if (RWKnownSections[i].Id == _id)
                {
					info = RWKnownSections[i];
					return true;
                }
            }
			return false;
		}

		public static bool RWIsSectionContainer(RWSectionId _id)
        {
			RWSectionInfo info;
			if (RWGetSectionInfo(_id, out info))
            {
				return info.Container;
            }
			return false;
        }

		public static bool RWGetSectionShortName(RWSectionId _id, out string _dest)
		{
			RWSectionInfo info;
			if (RWGetSectionInfo(_id, out info))
			{
				_dest = info.Name;
				return true;
			}
			else
			{
				_dest = string.Format("[Unknown: 0x{0}]", ((uint)_id).ToString("X"));
				return false;
			}
		}

		public static bool RWGetSectionName(RWSectionId _id, out string _dest)
		{
			string prefix = "RW_SECTION_";
			if (RWGetSectionShortName(_id, out _dest))
			{
				_dest = prefix + _dest;
				return true;
			}
			else
			{
				return false;
			}
		}

		public static string RWGetSectionName(RWSectionId _id)
        {
			string dest;
			RWGetSectionName(_id, out dest);
			return dest;
        }
	}
}
