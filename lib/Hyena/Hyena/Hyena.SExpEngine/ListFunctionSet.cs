//
// ListFunctionSet.cs
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

namespace Hyena.SExpEngine
{
    public class ListFunctionSet : FunctionSet
    {
        // FIXME: Why is this here? --Aaron
        //
        // private TreeNode EvaluateList(TreeNode node)
        // {
        //    TreeNode list = new TreeNode();
        //
        //    foreach(TreeNode child in node.Children) {
        //        list.AddChild(Evaluate(child));
        //    }
        //
        //    return list;
        // }

        bool IsList(TreeNode node)
        {
            return !(node is LiteralNodeBase) && !(node is FunctionNode);
        }

        public void CheckList(TreeNode node)
        {
            if(!IsList(node)) {
                throw new ArgumentException("argument must be a list");
            }
        }

        [Function("is-list")]
        public virtual TreeNode OnIsList(TreeNode [] args)
        {
            return new BooleanLiteral(IsList(args[0]));
        }

        [Function("item-at")]
        public virtual TreeNode OnItemAt(TreeNode [] args)
        {
			var list = Evaluate(args[0]);
            CheckList(list);

			var node = Evaluate(args[1]);
			if (!(node is IntLiteral)) {
                throw new ArgumentException("argument must be an index");
            }

			var index = (node as IntLiteral).Value;
			return Evaluate(list.Children[index]);
        }

        [Function("remove-at")]
        public virtual TreeNode OnRemoveAt(TreeNode [] args)
        {
			var list = Evaluate(args[0]);
            CheckList(list);

			var node = Evaluate(args[1]);
			if (!(node is IntLiteral)) {
                throw new ArgumentException("argument must be an index");
            }

			var index = (node as IntLiteral).Value;
			list.Children.RemoveAt(index);

            return list;
        }

        [Function("remove")]
        public virtual TreeNode OnRemove(TreeNode [] args)
        {
			var list = Evaluate(args[0]);
            CheckList(list);

			var node = Evaluate(args[1]);

            foreach(var compare_node in list.Children) {
                if(((IntLiteral)CompareFunctionSet.Compare(Evaluator, node, compare_node)).Value == 0) {
                    list.Children.Remove(compare_node);
                    break;
                }
            }

            return list;
        }

        [Function("append")]
        public virtual TreeNode OnAppend(TreeNode [] args)
        {
			var list = Evaluate(args[0]);
            CheckList(list);
            list.Children.Add(Evaluate(args[1]));
            return list;
        }

        [Function("prepend")]
        public virtual TreeNode OnPrepend(TreeNode [] args)
        {
			var list = Evaluate(args[0]);
            CheckList(list);
            list.Children.Insert(0, Evaluate(args[1]));
            return list;
        }

        [Function("insert")]
        public virtual TreeNode OnInsert(TreeNode [] args)
        {
			var list = Evaluate(args[0]);
            CheckList(list);

			var node = Evaluate(args[1]);
			if (!(node is IntLiteral)) {
                throw new ArgumentException("argument must be an index");
            }

			var index = (node as IntLiteral).Value;
			list.Children.Insert(index, Evaluate(args[2]));

            return list;
        }

        [Function("foreach")]
        public virtual TreeNode OnForeach(TreeNode [] args)
        {
			var list = Evaluate(args[1]);
            CheckList(list);
            var item_variable = (FunctionNode)args[0];
			var function = args[2];

			var self = args[0].Parent.Children[0];
            self.Parent.RegisterFunction(item_variable.Function, item_variable);

            foreach(var child in list.Children) {
                item_variable.Body = child;

                try {
                    if(function is FunctionNode) {
						var function_impl = Evaluator.ResolveFunction(function as FunctionNode);
                        function_impl.Evaluate(Evaluator, new TreeNode [] { child });
                    } else {
                        Evaluate(function);
                    }
                } catch(Exception e) {
                    if(ControlFunctionSet.BreakHandler(e)) {
                        break;
                    }
                }
            }

            return new VoidLiteral();
        }
    }
}
