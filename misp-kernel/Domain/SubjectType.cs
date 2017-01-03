using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
    public class SubjectType
    {
        public static SubjectType ALL = new SubjectType("All categories");

        public static SubjectType DEFAULT = new SubjectType("Default");

        public static SubjectType MODEL = new SubjectType("Model");

        public static SubjectType INPUT_TABLE = new SubjectType("Input Table");

        public static SubjectType REPORT = new SubjectType("Report");

        public static SubjectType TARGET = new SubjectType("Target");

        public static SubjectType TRANSFORMATION_TREE = new SubjectType("Transformation Tree");

        public static SubjectType CALCULATED_MEASURE = new SubjectType("Calculated Measure");

        public static SubjectType MEASURE = new SubjectType("Measure");

        public static SubjectType DESIGN = new SubjectType("Design");

        public static SubjectType AUTOMATIC_SOURCING = new SubjectType("Automatic Sourcing");

        public static SubjectType STRUCTURED_REPORT = new SubjectType("Structured Report");

        public static SubjectType PRESENTATION = new SubjectType("Presentation");

        public static SubjectType RECONCILIATION = new SubjectType("Reconciliation");

        public static SubjectType COMBINED_TRANSFORMATION_TREE = new SubjectType("Combined Transformation Tree");

        public static SubjectType AUTOMATIC_TARGET = new SubjectType("Automatic Target");

        public static SubjectType AUTOMATIC_GRID = new SubjectType("Automatic Grid");

        public static SubjectType AUTOMATIC_ENRICHMENT_TABLE = new SubjectType("Automatic Enrichment Table");

        public static SubjectType POSTING_GRID = new SubjectType("Posting Grid");

        public static SubjectType AUTOMATIC_POSTING_GRID = new SubjectType("Automatic Posting Grid");

        public static SubjectType GRID = new SubjectType("Grid");

        public static SubjectType INPUT_GRID = new SubjectType("Input Grid");

        public static SubjectType REPORT_GRID = new SubjectType("Report Grid");

        

        public static SubjectType RECONCILIATION_FILTER = new SubjectType("Reconciliation Filter");

        public static SubjectType USER = new SubjectType("User");

        public static SubjectType PROFIL = new SubjectType("Profil");

        public static SubjectType ROLE = new SubjectType("Role");

        public static SubjectType GROUP = new SubjectType("Group");

        public String label { get; set; }

        private SubjectType(String label)
        {
            this.label = label;
        }

        public override string ToString()
        {
            return label;
        }

        public static SubjectType getByLabel(String label)
        {
            if (string.IsNullOrEmpty(label)) return null;
            if (DEFAULT.label.Equals(label)) return DEFAULT;
            if (MODEL.label.Equals(label)) return MODEL;
            if (INPUT_TABLE.label.Equals(label)) return INPUT_TABLE;
            if (REPORT.label.Equals(label)) return REPORT;
            if (TARGET.label.Equals(label)) return TARGET;
            if (TRANSFORMATION_TREE.label.Equals(label)) return TRANSFORMATION_TREE;
            if (CALCULATED_MEASURE.label.Equals(label)) return CALCULATED_MEASURE;
            if (MEASURE.label.Equals(label)) return MEASURE;
            if (DESIGN.label.Equals(label)) return DESIGN;
            if (AUTOMATIC_SOURCING.label.Equals(label)) return AUTOMATIC_SOURCING;
            if (STRUCTURED_REPORT.label.Equals(label)) return STRUCTURED_REPORT;
            if (PRESENTATION.label.Equals(label)) return PRESENTATION;
            if (RECONCILIATION.label.Equals(label)) return RECONCILIATION;
            if (COMBINED_TRANSFORMATION_TREE.label.Equals(label)) return COMBINED_TRANSFORMATION_TREE;
            if (AUTOMATIC_TARGET.label.Equals(label)) return AUTOMATIC_TARGET;
            if (AUTOMATIC_GRID.label.Equals(label)) return AUTOMATIC_GRID;
            if (POSTING_GRID.label.Equals(label)) return POSTING_GRID;
            if (AUTOMATIC_POSTING_GRID.label.Equals(label)) return AUTOMATIC_POSTING_GRID;
            if (AUTOMATIC_ENRICHMENT_TABLE.label.Equals(label)) return AUTOMATIC_ENRICHMENT_TABLE;
            if (GROUP.label.Equals(label)) return GROUP;
            if (USER.label.Equals(label)) return USER;
            if (PROFIL.label.Equals(label)) return PROFIL;
            if (ROLE.label.Equals(label)) return ROLE;
            if (ALL.label.Equals(label)) return ALL;
            if (GRID.label.Equals(label)) return GRID;
            if (INPUT_GRID.label.Equals(label)) return INPUT_GRID;
            if (REPORT_GRID.label.Equals(label)) return REPORT_GRID;
            return null;
        }

        public Boolean isAll()
        {
            return ALL.label.Equals(label);
        }

        public static ObservableCollection<SubjectType> GetCategories()
        {
            ObservableCollection<SubjectType> types = new ObservableCollection<SubjectType>();
            types.Add(ALL);
            types.Add(AUTOMATIC_SOURCING);
            types.Add(CALCULATED_MEASURE);
            types.Add(COMBINED_TRANSFORMATION_TREE);            
            types.Add(DESIGN);
            types.Add(INPUT_TABLE);
            types.Add(MEASURE);
            types.Add(REPORT);
            types.Add(TARGET);
            types.Add(STRUCTURED_REPORT);
            types.Add(TRANSFORMATION_TREE);
			types.Add(AUTOMATIC_GRID);
            types.Add(GRID);
            types.Add(INPUT_GRID);
            types.Add(REPORT_GRID);
            types.Add(RECONCILIATION);
            types.Add(USER);
            types.Add(PROFIL);
            types.Add(ROLE);
            return types;
        }
        
    }
}
