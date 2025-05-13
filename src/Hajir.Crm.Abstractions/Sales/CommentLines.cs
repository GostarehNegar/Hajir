using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Hajir.Crm.Sales
{
    public class InputTemplate
    {
        public string Caption { get; set; }
        public string Value { get; set; }

    }
    public class CommentLine
    {
        //private readonly string template;
        //private readonly string value;

        public CommentLine(string template, string value)
        {
            this.Template = template;
            this.Value = value;
        }
        public string Value { get; private set; }
        public string Template { get; private set; }
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is CommentLine g)
            {
                return g.Template == this.Template;
            }
            return false;
        }
        public void SetTemplateValues(InputTemplate[] values)
        {
            if (values != null)
            {
                this.Value = this.Template;
                values.ToList()
                    .ForEach(x =>
                    {
                        if (!string.IsNullOrWhiteSpace(x.Value))
                        {

                            this.Value = this.Value.Replace("{" + x.Caption + "}", x.Value);
                        }
                    });
            }
        }
        public InputTemplate[] GetInputs()
        {
            var pos = 0;
            var result = new List<InputTemplate>();
            while (true)
            {
                var start = this.Template.IndexOf("{", pos);
                if (start < 0)
                    break;
                var fin = this.Template.IndexOf("}", start);
                if (fin < start)
                {
                    break;
                }
                var mask = this.Template.Substring(start, fin - start + 1);
                var caption = this.Template.Substring(start + 1, fin - start - 1);
                pos = start + 1;
                result.Add(new InputTemplate
                {
                    Caption = caption
                });
            }
            return result.ToArray();
        }
    }
    public class CommentLines
    {
        private List<CommentLine> comments;
        private HashSet<CommentLine> selected;
        private List<CommentLine> allComments;
        public static IEnumerable<CommentLine> AllLines => HajirCrmConstants.QuoteComments.All().Select(x => new CommentLine(x, x));

        public CommentLine Find(string value)
        {
            return AllLines.OrderBy(x => HajirUtils.Instance.LevenshteinDistance(value, x.Template))
                .FirstOrDefault();
        }
        public CommentLines(string value)
        {
            this.allComments = AllLines.ToList();
            this.comments = new List<CommentLine>();
            value = value ?? "";
            this.Selected = new HashSet<CommentLine>();
            var _comments = value.Split('\n')
                .Select(x => x.Replace("\r", ""))
                .Where(x => !string.IsNullOrWhiteSpace(x));
            foreach (var comment in _comments)
            {

                var line = Find(comment);
                this.comments.Add(new CommentLine(line?.Template ?? comment, comment));
                this.Selected.Add(new CommentLine(line?.Template ?? comment, comment));
            }
            this.allComments = this.Selected.ToList();
            var remain = AllLines.Where(x => !this.allComments.Any(y => x.Template == y.Template))
                .ToArray();
            this.allComments.AddRange(remain);
        }
        public HashSet<CommentLine> Selected { get; set; } = new HashSet<CommentLine>();

        public IEnumerable<CommentLine> Selectable
        {
            get { return allComments; }
        }
        public void AddLine(CommentLine line)
        {
            this.comments.Add(line);

        }
        public void MoveUp(CommentLine line)
        {
            var idx = this.allComments.IndexOf(line);
            if (idx > 0)
            {
                this.allComments.RemoveAt(idx);
                this.allComments.Insert(idx - 1, line);
            }


        }
        public string Text
        {
            get
            {
                var _selected = this.allComments.Where(x => this.Selected.Contains(x)).ToArray();
                return String.Join("\r\n", _selected.Select(x => x.Value));
            }
            set
            {

            }
        }





    }
}

