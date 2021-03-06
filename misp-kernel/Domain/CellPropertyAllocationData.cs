﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class CellPropertyAllocationData : Persistent
    {

        public enum AllocationType { Scope2Scope, Linear, Template, NoAllocation, Reference }


         /// <summary>
        /// Construit une npouvelle instance de CellPropertyAllocationData.
        /// </summary>
        public CellPropertyAllocationData() {
            type = AllocationType.NoAllocation.ToString();
            active = true;
            considerCell = true;
            showGridInShortcut = true;
        }

        public string type { get; set; }
        
        public Measure measureRef { get; set; }

        public bool active { get; set; }

        public TransformationTree allocationTree { get; set; }

        public bool showGridInShortcut { get; set; }

        public bool considerCell { get; set; }

        public CellPropertyAllocationData GetCopy()
        {
            CellPropertyAllocationData data = new CellPropertyAllocationData();
            data.type = this.type;
            data.active = this.active;
            data.measureRef = this.measureRef;
            data.allocationTree = this.allocationTree;
            data.showGridInShortcut = this.showGridInShortcut;
            return data;
        }

    }
}
