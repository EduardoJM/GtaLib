using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderWareLib
{
    public enum RWSectionId : uint
    {
        RW_SECTION_INVALID = 0,
        /// <summary>
        /// Same as Data.
        /// </summary>
        RW_SECTION_STRUCT = 1,
        RW_SECTION_STRING = 2,
        RW_SECTION_EXTENSION = 3,
        RW_SECTION_CAMERA = 5,
        RW_SECTION_TEXTURE = 6,
        RW_SECTION_MATERIAL = 7,
        RW_SECTION_MATERIALLIST = 8,
        RW_SECTION_FRAMELIST = 14,
        RW_SECTION_GEOMETRY = 15,
        RW_SECTION_CLUMP = 16,
        RW_SECTION_ATOMIC = 20,
        RW_SECTION_GEOMETRYLIST = 26,
        /// <summary>
        /// Same as Bin Mesh PLG.
        /// </summary>
        RW_SECTION_MATERIALSPLIT = 1294,
        RW_SECTION_FRAME = 39056126,

        RW_SECTION_SKIN_PLG = 0x0116,
        RW_SECTION_HANIM_PLG = 0x011E,
        RW_SECTION_SKY_MIPMAP_VAL = 0x0110,
        RW_SECTION_MATERIAL_EFFECTS_PLG = 0x0120,
        RW_SECTION_SPECULAR_MATERIAL = 0x0253F2F6,
        RW_SECTION_NIGHT_VERTEX_COLORS = 0x0253F2F9,
        RW_SECTION_REFLECTION_MATERIAL = 0x0253F2FC,
        RW_SECTION_2DFX = 0x0253F2F8,
        RW_SECTION_MESH_EXTENSION = 0x0253F2FD,
        RW_SECTION_RIGHT_TO_RENDER = 0x001F,
        RW_SECTION_COLLISION_MODEL = 0x0253F2FA,

        RW_SECTION_TEXTUREDICTIONARY = 0x16,
        RW_SECTION_TEXTURENATIVE = 0x15,

        RW_SECTION_UV_ANIM_DICTIONARY = 0x2B,
        RW_SECTION_UV_ANIM_PLG = 0x135,
        RW_SECTION_MORPH_PLG = 0x105,
        RW_SECTION_PIPELINE_SET = 0x253F2F3,
    }
}
