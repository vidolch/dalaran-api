// Copyright (c) Vidol Chalamov.
// See the LICENSE file in the project root for more information.

namespace Dalaran.Core.Domain.Code
{
    using System.Text;

    public class Script
    {
        private readonly StringBuilder sb = new StringBuilder();

        private int tabs = 0;

        private bool addTabs = true;

        public Script()
        {
            this.sb = new StringBuilder();
        }

        public Script AddLine(string sql)
        {
            this.Add(sql).AddLine();

            return this;
        }

        public Script Add(string sql)
        {
            if (this.addTabs)
            {
                this.sb.Append(this.GetTabs()).Append(sql);
            }
            else
            {
                this.sb.Append(sql);
            }

            this.addTabs = false;
            return this;
        }

        public Script AddLine()
        {
            this.sb.AppendLine();
            this.addTabs = true;
            return this;
        }

        public override string ToString()
        {
            return this.sb.ToString();
        }

        public Script IncreaseTabs(int with = 1)
        {
            this.tabs += with;
            return this;
        }

        public Script DecreaseTabs(int with = 1)
        {
            this.tabs = this.tabs != 0 ? this.tabs -= with : this.tabs;
            return this;
        }

        private string GetTabs()
        {
            return new string('\t', this.tabs);
        }
    }
}
