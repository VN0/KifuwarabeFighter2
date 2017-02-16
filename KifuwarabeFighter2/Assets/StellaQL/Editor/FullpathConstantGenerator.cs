﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Animations;

namespace StellaQL
{
    public abstract class FullpathConstantGenerator
    {
        public static void WriteCshapScript(AnimatorController ac, StringBuilder info_message)
        {
            AconStateNameScanner aconScanner = new AconStateNameScanner();
            aconScanner.ScanAnimatorController(ac, info_message);

            StringBuilder contents = new StringBuilder();

            // 変換例:
            // Conversion example:

            // 「Main_Char3」は「Main_Char3」(同じ)
            // "Main_Char3" is "Main_Char3" (same)

            // 「BattleFloor_char@arm@finger」は「Battolefloor_Chararmfinger」
            // "BattleFloor_char@arm@finger" is "Battolefloor_Chararmfinger"
            string className = FullpathConstantGenerator.String_to36_pascalCase(ac.name, "_", "_");

            string abstractClassName = className + "_AbstractAControl";

            contents.AppendLine("using System.Collections.Generic;");
            contents.AppendLine();
            contents.Append("namespace StellaQL.Acons."); contents.AppendLine(className);
            contents.AppendLine("{");
            contents.AppendLine("    /// <summary>");
            contents.AppendLine("    /// This file was automatically generated.");
            contents.Append("    /// It was created by ["); contents.Append(StateCmdline.BUTTON_LABEL_GENERATE_FULLPATH); contents.AppendLine("] button.");
            contents.AppendLine("    /// </summary>");
            contents.Append("    public abstract class "); contents.Append(abstractClassName);
            contents.AppendLine(" : AbstractAControl");
            contents.AppendLine("    {");
            List<string> fullpaths = new List<string>(aconScanner.FullpathSet);
            fullpaths.Sort();
            if(0< fullpaths.Count)
            {
                contents.Append("        public const string");
                foreach (string fullpath in fullpaths)
                {
                    // 先に改行を持ってくる。最後のセミコロンを付ける処理を簡単にする。
                    // Bring newlines first. Make the process of attaching the final semicolon easier.
                    contents.AppendLine(); 

                    contents.Append("            ");
                    contents.Append(FullpathConstantGenerator.String_split_toUppercaseAlphabetFigureOnly_join(fullpath, ".", "_"));
                    contents.Append(@" = """);
                    contents.Append(fullpath);

                    // 改行は最後ではなく、最初に付けておく。
                    // Line breaks are not the last, but attach them first.
                    contents.Append(@""",");
                }

                // 最後のコンマを削る。
                // Cut the last comma.
                contents.Length--;

                // 代わりにセミコロンを追加する。
                // Add a semicolon instead.
                contents.AppendLine(@"; // semi colon");

                contents.AppendLine();
            }

            contents.Append("        public "); contents.Append(abstractClassName); contents.AppendLine("()");
            contents.AppendLine("        {");
            contents.AppendLine("            Code.Register(StateHash_to_record, new List<AcStateRecordable>()");
            contents.AppendLine("            {");

            foreach (string fullpath in fullpaths)
            {
                contents.Append("                new DefaultAcState( ");
                contents.Append(FullpathConstantGenerator.String_split_toUppercaseAlphabetFigureOnly_join(fullpath, ".", "_"));
                contents.AppendLine("),");
            }

            contents.AppendLine("            });");
            contents.AppendLine("        }");
            contents.AppendLine("    }");
            contents.AppendLine("}");

            StellaQLWriter.Write(StellaQLWriter.Filepath_GenerateFullpathConstCs(ac), contents, info_message);
        }

        /// <summary>
        /// アルファベット２６文字と、数字１０文字だけに詰めます。
        /// It fills in only 26 letters of the alphabet and 10 numeric characters.
        /// 
        /// 英字・数字に変換できない文字は無視します。
        /// Ignore characters that can not be converted to alphabetic or numeric characters.
        /// 
        /// パスカルケースにします。（white.alpaca -> WhiteAlpaca）
        /// I will make it Pascal case. (White.alpaca -> WhiteAlpaca)
        /// 
        /// 通称 To36
        /// Commonly designated "To36"
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string String_to36_pascalCase(string source1, string splitSeparator, string joinSeparator)
        {
            string[] tokens = source1.Split(new string[] { splitSeparator }, StringSplitOptions.None);
            for (int iToken = 0; iToken < tokens.Length; iToken++)
            {
                StringBuilder sb = new StringBuilder();
                string token = tokens[iToken];
                for (int caret = 0; caret < token.Length; caret++)
                {
                    // 先頭の文字
                    // First character
                    if (sb.Length == 0)
                    {
                        // 大文字と数字はそのまま追加
                        // Capital letters and numbers are added as they are
                        if (Char.IsUpper(token[caret]) || Char.IsDigit(token[caret])) {
                            sb.Append(token[caret]);
                        }
                        // 小文字は大文字にして追加
                        // Add lowercase letters in uppercase letters
                        else if (Char.IsLower(token[caret])) {
                            sb.Append(Char.ToUpper(token[caret]));
                        }
                        // その他の文字は無視
                        // Ignore other characters
                    }
                    //先頭以降の文字
                    // Characters after the first character
                    else
                    {
                        // 小文字と数字はそのまま追加
                        // Add lowercase letters and numbers directly
                        if (Char.IsLower(token[caret]) || Char.IsDigit(token[caret])) {
                            sb.Append(token[caret]);
                        }
                        // 大文字は小文字にして追加
                        // Add capital letters in lowercase letters
                        else if (Char.IsUpper(token[caret])) {
                            sb.Append(Char.ToLower(token[caret]));
                        }
                        // その他の文字は無視
                        // Ignore other characters
                    }
                }
                tokens[iToken] = sb.ToString(); // 2017-02-16 追加Add
            }
            return string.Join(joinSeparator, tokens);
        }

        /// <summary>
        /// 大文字アルファベット２６文字と、数字１０文字だけに詰めます。
        /// Capitalize only 26 letters and 10 letters of the alphabet.
        /// 
        /// 英字・数字に変換できない文字は無視します。
        /// Ignore characters that can not be converted to alphabetic or numeric characters.
        /// 
        /// 通称 To36
        /// Commonly designated "To36"
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string String_split_toUppercaseAlphabetFigureOnly_join(string source1, string splitSeparator, string joinSeparator)
        {
            string[] tokens = source1.Split(new string[] { splitSeparator }, StringSplitOptions.None);
            for (int iToken = 0; iToken < tokens.Length; iToken++)
            {
                string token = tokens[iToken];
                StringBuilder sb = new StringBuilder();
                for (int caret = 0; caret < token.Length; caret++)
                {
                    // 大文字と数字はそのまま追加
                    // Capital letters and numbers are added as they are
                    if (Char.IsUpper(token[caret]) || Char.IsDigit(token[caret])) { sb.Append(token[caret]); }
                    // 小文字は大文字にして追加
                    // Add lowercase letters in uppercase letters
                    else if (Char.IsLower(token[caret])) { sb.Append(Char.ToUpper(token[caret])); }
                    // その他の文字は無視
                    // Ignore other characters
                }
                tokens[iToken] = sb.ToString();
            }
            return string.Join(joinSeparator, tokens);
        }

        /// <summary>
        /// 大文字アルファベット２６文字と、数字１０文字だけに詰めます。
        /// Capitalize only 26 letters and 10 letters of the alphabet.
        /// 
        /// 英字・数字に変換できない文字は無視します。
        /// Ignore characters that can not be converted to alphabetic or numeric characters.
        /// 
        /// 通称 To36
        /// Commonly designated "To36"
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string String_to_UppercaseAlphabetFigureOnly(string source)
        {
            StringBuilder sb = new StringBuilder();
            for (int caret = 0; caret < source.Length; caret++)
            {
                // 大文字と数字はそのまま追加
                // Capital letters and numbers are added as they are
                if (Char.IsUpper(source[caret]) || Char.IsDigit(source[caret])) { sb.Append(source[caret]); }
                // 小文字は大文字にして追加
                // Add lowercase letters in uppercase letters
                else if (Char.IsLower(source[caret])) { sb.Append(Char.ToUpper(source[caret])); }
                // その他の文字は無視
                // Ignore other characters
            }
            return sb.ToString();
        }

        public static string String_to_AlphabetFigureOnly(string source)
        {
            StringBuilder sb = new StringBuilder();
            for (int caret = 0; caret < source.Length; caret++)
            {
                // 大文字、小文字と数字はそのまま追加
                // Add capital letters, lower case letters and numbers directly
                if (Char.IsUpper(source[caret]) || Char.IsLower(source[caret]) || Char.IsDigit(source[caret])) { sb.Append(source[caret]); }
                // その他の文字は無視
                // Ignore other characters
            }
            return sb.ToString();
        }

    }
}
