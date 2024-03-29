//
// TreeNode.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2007-2008 Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;

namespace Hyena.SExpEngine
{
	public class TreeNode
    {
		readonly Dictionary<string, FunctionNode> functions = new Dictionary<string, FunctionNode>();
		int column;

		public TreeNode ()
        {
        }

        public TreeNode(TreeNode parent)
        {
            Parent = parent;
            parent.AddChild(this);
        }

        internal void CopyFunctionsFrom(TreeNode node)
        {
            foreach(var function in node.Functions) {
                RegisterFunction(function.Key, function.Value);
            }
        }

        internal void RegisterFunction(string name, object value)
        {
            if(functions.ContainsKey(name)) {
                functions[name] = new FunctionNode(name, value);
            } else {
                functions.Add(name, new FunctionNode(name, value));
            }
        }

        internal void RegisterFunction(string name, FunctionNode function)
        {
            if(functions.ContainsKey(name)) {
                functions[name] = function;
            } else {
                functions.Add(name, function);
            }
        }

        public TreeNode Flatten()
        {
            var result_node = new TreeNode();
            Flatten(result_node, this);

            return result_node.ChildCount == 1 ? result_node.Children[0] : result_node;
        }

        void Flatten(TreeNode result_node, TreeNode node)
        {
            if(node == null) {
                return;
            }

            if(!node.HasChildren && !(node is VoidLiteral)) {
                result_node.AddChild(node);
                return;
            }

            foreach(var child_node in node.Children) {
                Flatten(result_node, child_node);
            }
        }

        public void AddChild(TreeNode child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public TreeNode Parent { get; set; }

        public int ChildCount {
            get { return Children.Count; }
        }

        public bool HasChildren {
            get { return ChildCount > 0; }
        }

        public int Line { get; set; }

        public int Column {
            get { return column; }
            set { column = value; }
        }

        public List<TreeNode> Children { get; } = new List<TreeNode>();

		public IDictionary<string, FunctionNode> Functions {
            get { return functions; }
        }

        public int FunctionCount {
            get { return functions.Count; }
        }

        public bool Empty {
            get { return ChildCount == 0 && !(this is LiteralNodeBase) && !(this is FunctionNode); }
        }

        public void Dump()
        {
            DumpTree(this);
        }

        public TreeNode FindRootNode()
        {
			var shift_node = this;

            while(shift_node.Parent != null) {
                shift_node = shift_node.Parent;
            }

            return shift_node;
        }

        public static void DumpTree(TreeNode node)
        {
            DumpTree(node, 0);
        }

        static void DumpTree(TreeNode node, int depth)
        {
            if(node is LiteralNodeBase || node is FunctionNode) {
                PrintIndent(depth, node);
            } else if(node != null) {
                int i = 0;
                Console.Write("{0}+ [", string.Empty.PadLeft(depth * 2, ' '));
                foreach(var function in node.Functions) {
                    Console.Write("{0}{1}", function.Key, i++ < node.FunctionCount - 1 ? ", " : string.Empty);
                }
                Console.WriteLine("]");
                foreach(var child in node.Children) {
                    DumpTree(child, depth + 1);
                }
            }
        }

        static void PrintIndent(int depth, TreeNode node)
        {
            Console.Write(string.Empty.PadLeft(depth * 2, ' '));

            if(node is FunctionNode) {
                Console.WriteLine((node as FunctionNode).Function);
            } else {
                Console.WriteLine(node);
            }
        }
    }
}
