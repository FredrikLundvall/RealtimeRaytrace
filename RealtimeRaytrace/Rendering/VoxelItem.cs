using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelRaytrace
{
    public class VoxelItem : Placeable, Moveable, Physical
    {
        private int id = 0;
        private bool isIndexedByPosition;

        private Constructable constructable;
        private Position position;
        private Material material;

        public VoxelItem(Position position, bool isIndexedByPosition)
        {
            this.isIndexedByPosition = isIndexedByPosition;
            SetPosition(position);
        }

        public VoxelItem(Position position)
            : this(position, false)
        {
        }

        public void InitId()
        {
            if ((id == 0) && (constructable != null))
                id = constructable.GetNextId();
        }

        public int GetId()
        {
            return id;
        }

        public void SetIsIndexedByPosition(bool isIndexedByPosition)
        {
            this.isIndexedByPosition = isIndexedByPosition;
        }

        public bool GetIsIndexedByPosition()
        {
            return isIndexedByPosition;
        }

        public void SetConstructable(Constructable constructable)
        {
            this.constructable = constructable;
        }

        public Constructable GetConstructable()
        {
            return this.constructable;
        }

        public bool IsMoveable()
        {
            return true;
        }

        public Moveable GetMoveable()
        {
            return this;
        }

        public bool IsTurnable()
        {
            return false;
        }

        public Turnable GetTurnable()
        {
            return null;
        }

        public bool IsViewable()
        {
            return false;
        }

        public Viewable GetViewable()
        {
            return null;
        }

        public bool IsPhysical()
        {
            return true;
        }

        public Physical GetPhysical()
        {
            return this;
        }

        public Placeable GetPlaceable()
        {
            return this;
        }

        public Position GetPosition()
        {
            return position;
        }

        public void SetPosition(Position position)
        {
            this.position = position;
        }

        public Material GetMaterial()
        {
            return material;
        }

        public void SetMaterial(Material material)
        {
            this.material = material;
        } 
        
        public void MovePosition(Position position)
        {
            //If the key is changed in the dictionary we have to call the: MoveMoveable(Moveable,Position)
            constructable.ReIndexMoveable(this, position);
        }

    }
}
