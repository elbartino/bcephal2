using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Domain
{
  public class Bracket
  {

        public static Bracket OPEN_BRACKET = new Bracket("(");

        public static Bracket CLOSE_BRACKET = new Bracket(")");

        public static Bracket OPEN_2BRACKET = new Bracket("((");

        public static Bracket CLOSE_2BRACKET = new Bracket("))");

        public String sign { get; set; }
        public String label { get; set; }

        private Bracket(String name)
        {
            this.sign = sign;
            this.label = label;
        }

        public static Bracket getBySign(String sign)
        {
            if (string.IsNullOrEmpty(sign)) return null;
            if (OPEN_BRACKET.sign.Equals(sign)) return OPEN_BRACKET;
            if (CLOSE_BRACKET.sign.Equals(sign)) return CLOSE_BRACKET;
            if (OPEN_2BRACKET.sign.Equals(sign)) return OPEN_2BRACKET;
            if (CLOSE_2BRACKET.sign.Equals(sign)) return CLOSE_2BRACKET;
            return null;
        }

        public static Bracket getByLabel(String label)
        {
            if (string.IsNullOrEmpty(label)) return null;
            if (OPEN_BRACKET.sign.Equals(label)) return OPEN_BRACKET;
            if (CLOSE_BRACKET.sign.Equals(label)) return CLOSE_BRACKET;
            if (OPEN_2BRACKET.sign.Equals(label)) return OPEN_2BRACKET;
            if (CLOSE_2BRACKET.sign.Equals(label)) return CLOSE_2BRACKET;
            return null;
        }
    }
}
