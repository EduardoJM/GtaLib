using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RenderWareLib;
using RenderWareLib.Mathematics;

namespace GtaLib.DFF
{
    public class DFFFrame
    {
        public RWMatrix4 ModelMatrix { get; set; }

        public RWMatrix4 LocalTransformMatrix { get; set; }

        public DFFFrame Parent { get; set; }

        public uint Flags { get; set; }

        public DFFBone Bone { get; set; }

        public string Name { get; set; }

        public List<DFFFrame> Children { get; private set; } = new List<DFFFrame>();

        public DFFFrame(RWMatrix4 modelMatrix)
        {
            ModelMatrix = modelMatrix;
            LocalTransformMatrix = null;
            Parent = null;
            Flags = 0;
            Bone = null;
        }

        public DFFFrame GetChildByBoneID(int id, bool recurse)
        {
            for (int i = 0; i < Children.Count; i += 1)
            {
                DFFFrame child = Children[i];
                if (child.Bone != null  && child.Bone.Index == id)
                {
                    return child;
                }
                if (recurse)
                {
                    child = child.GetChildByBoneID(id, true);
                    if (child != null)
                    {
                        return child;
                    }
                }
            }
            return null;
        }

        public void Reparent(DFFFrame frame)
        {
            if (frame != null && Parent != null)
            {
                throw new DFFException("Attempt to reparent a DFFFrame which still has a parent! Remove it from it's old parent first.");
            }
            Parent = frame;
        }

        public RWMatrix4 GetLocalTransformationMatrix()
        {
            EnsureValidLocalTransformationMatrix();
            return this.LocalTransformMatrix;
        }

        public void EnsureValidLocalTransformationMatrix()
        {
            if (LocalTransformMatrix == null)
            {
                if (Parent != null)
                {
                    LocalTransformMatrix = new RWMatrix4(ModelMatrix * Parent.GetLocalTransformationMatrix());
                }
                else
                {
                    LocalTransformMatrix = new RWMatrix4(ModelMatrix);
                }
            }
        }

        public void InvalidateLocalTransformationMatrix()
        {
            if (LocalTransformMatrix != null)
            {
                LocalTransformMatrix = null;
            }
            for (int i = 0; i < Children.Count; i += 1)
            {
                DFFFrame child = Children[i];
                child.InvalidateLocalTransformationMatrix();
            }
        }

        public DFFFrame GetChild(int index)
        {
            if (index < 0 || index >= Children.Count)
            {
                throw new IndexOutOfRangeException("The children index is out of range");
            }
            return Children[index];
        }

        public int IndexOf(DFFFrame child)
        {
            return Children.IndexOf(child);
        }

        public void ClearChildren()
        {
            for (int i = 0; i < Children.Count; i += 1)
            {
                Children[i].Reparent(null);
            }
            Children.Clear();
        }

        public void RemoveChild(DFFFrame child)
        {
            child.Reparent(null);
            Children.Remove(child);
        }

        public DFFFrame GetChild(string name)
        {
            for (int i = 0; i < Children.Count; i += 1)
            {
                DFFFrame child = Children[i];
                if (child.Name == name)
                {
                    return child;
                }
            }
            return null;
        }

        public void AddChild(DFFFrame frame)
        {
            Children.Add(frame);
            frame.Reparent(this);
        }
    }
}
