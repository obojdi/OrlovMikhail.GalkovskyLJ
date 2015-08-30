﻿using OrlovMikhail.LJ.Grabber;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrlovMikhail.LJ.BookWriter;
using OrlovMikhail.Tools;

namespace OrlovMikhail.LJ.BookWriter.AsciiDoc
{
    public class AsciiDocBookWriter : BookWriterBase, IBookWriter
    {
        StreamWriter _sr;

        #region ctor and write

        public AsciiDocBookWriter(DirectoryInfoBase root, FileInfoBase path)
            : base(root, path, new AsciiDocTextPreparator())
        {
            _sr = new StreamWriter(path.FullName, append: false, encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            _sr.AutoFlush = true;
        }

        public override void Dispose()
        {
            if (_sr != null)
            {
                _sr.Flush();
                _sr.Dispose();
                _sr = null;
            }
        }

        void P(string s) { _sr.Write(s); }
        void PL(string s) { _sr.WriteLine(s); }
        #endregion

        public override void ThreadBegin()
        {
            PL("");
            PL("''''");
        }

        public override void EntryEnd() { PL(""); }
        public override void CommentEnd() { PL(""); }

        public override void EntryHeader(DateTime dateTime, long id, string subject, UserLite user, string posterUserpicRelativeLocation)
        {
            if (subject.Length == 0)
                subject = id.ToString();

            subject = subject.Trim();
            if (subject.EndsWith("."))
                subject = subject.Substring(0, subject.Length - 1);

            PL(String.Format("== {0}", subject));
            PL("");

            if (posterUserpicRelativeLocation != null)
                PL(String.Format("image:{0}[userpic, 40, 40]", posterUserpicRelativeLocation));
            PL(String.Format("{0:dd-MM-yyy HH:mm}", dateTime));
            PL("");
        }

        public override void CommentHeader(DateTime dateTime, long id, string subject, UserLite user, string commentUserpicRelativeLocation)
        {
            PL("");
            if (commentUserpicRelativeLocation != null)
                PL(String.Format("image:{0}[userpic, 40, 40]", commentUserpicRelativeLocation));

            PL(String.Format("*{0}, {1:dd-MM-yyy HH:mm}*", user.Username, dateTime));
            if (!String.IsNullOrWhiteSpace(subject))
                PL(String.Format("{0}", subject));

            PL("");
        }

        protected override void WriteImageInternal(string relativePath)
        {
            P("image::" + relativePath + "[]");
        }

        protected override void WritePreparedTextInternal(string preparedText)
        {
            string[] lines = SplitToLines(preparedText);
            for (int i = 0; i < lines.Length; i++)
            {
                P(lines[i]);
                if (i < lines.Length - 1)
                    PL("");
            }
        }

        protected override void WriteUsernameInternal(string username)
        {
            PL(String.Format("*{0}*", username));
        }

        protected override void WriteParagraphStartInternal() { PL(""); PL(""); }
        protected override void WriteLineBreakInternal() { PL(" +"); }
        protected override void WriteBoldStartInternal() { P("*"); }
        protected override void WriteBoldEndInternal() { P("*"); }
        protected override void WriteItalicStartInternal() { P("_"); }
        protected override void WriteItalicEndInternal() { P("_"); }

        public static string[] SplitToLines(string text, int lineWidth = 60)
        {
            List<string> ret = new List<string>();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (sb.Length >= lineWidth)
                {
                    char previous = sb[sb.Length - 1];
                    // We can't skip after 
                    bool canSkipHere = previous != ';';

                    if (canSkipHere && Char.IsWhiteSpace(c))
                    {
                        ret.Add(sb.ToString());
                        sb.Clear();
                        continue;
                    }
                }
                sb.Append(c);
            }

            if (sb.Length > 0)
                ret.Add(sb.ToString());

            return ret.ToArray();
        }
    }
}
