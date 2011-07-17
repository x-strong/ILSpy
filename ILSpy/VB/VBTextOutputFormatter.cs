﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.VB;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.VB
{
	/// <summary>
	/// Description of VBTextOutputFormatter.
	/// </summary>
	public class VBTextOutputFormatter : IOutputFormatter
	{
		readonly ITextOutput output;
		readonly Stack<AstNode> nodeStack = new Stack<AstNode>();
		
		public VBTextOutputFormatter(ITextOutput output)
		{
			if (output == null)
				throw new ArgumentNullException("output");
			this.output = output;
		}
		
		public void StartNode(AstNode node)
		{
//			var ranges = node.Annotation<List<ILRange>>();
//			if (ranges != null && ranges.Count > 0)
//			{
//				// find the ancestor that has method mapping as annotation
//				if (node.Ancestors != null && node.Ancestors.Count() > 0)
//				{
//					var n = node.Ancestors.FirstOrDefault(a => a.Annotation<MemberMapping>() != null);
//					if (n != null) {
//						MemberMapping mapping = n.Annotation<MemberMapping>();
//
//						// add all ranges
//						foreach (var range in ranges) {
//							mapping.MemberCodeMappings.Add(new SourceCodeMapping {
//							                               	ILInstructionOffset = range,
//							                               	SourceCodeLine = output.CurrentLine,
//							                               	MemberMapping = mapping
//							                               });
//						}
//					}
//				}
//			}
			
			nodeStack.Push(node);
		}
		
		public void EndNode(AstNode node)
		{
			if (nodeStack.Pop() != node)
				throw new InvalidOperationException();
		}
		
		public void WriteIdentifier(string identifier)
		{
			MemberReference memberRef = GetCurrentMemberReference();
			
			if (memberRef != null)
				output.WriteReference(identifier, memberRef);
			else
				output.Write(identifier);
		}

		MemberReference GetCurrentMemberReference()
		{
			AstNode node = nodeStack.Peek();
			MemberReference memberRef = node.Annotation<MemberReference>();
			// TODO : implement this!
//			if (memberRef == null && node.Role == AstNode.Roles.TargetExpression && (node.Parent is InvocationExpression || node.Parent is ObjectCreateExpression)) {
//				memberRef = node.Parent.Annotation<MemberReference>();
//			}
			return memberRef;
		}
		
		public void WriteKeyword(string keyword)
		{
			output.Write(keyword);
		}
		
		public void WriteToken(string token)
		{
			// Attach member reference to token only if there's no identifier in the current node.
			MemberReference memberRef = GetCurrentMemberReference();
			if (memberRef != null && nodeStack.Peek().GetChildByRole(AstNode.Roles.Identifier).IsNull)
				output.WriteReference(token, memberRef);
			else
				output.Write(token);
		}
		
		public void Space()
		{
			output.Write(' ');
		}
		
		public void Indent()
		{
			output.Indent();
		}
		
		public void Unindent()
		{
			output.Unindent();
		}
		
		public void NewLine()
		{
			output.WriteLine();
		}
		
		public void WriteComment(bool isDocumentation, string content)
		{
			if (isDocumentation)
				output.Write("'''");
			else
				output.Write("'");
			output.WriteLine(content);
		}
		
		public void MarkFoldStart()
		{
			output.MarkFoldStart();
		}
		
		public void MarkFoldEnd()
		{
			output.MarkFoldEnd();
		}
	}
}