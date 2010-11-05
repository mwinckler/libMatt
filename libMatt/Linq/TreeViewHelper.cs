using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace libMatt.Linq {
	public static class TreeViewHelper {

		/// <summary>
		/// Gets all checked TreeNodes in the specified TreeView.
		/// </summary>
		/// <param name="treeview"></param>
		/// <returns></returns>
		public static IEnumerable<TreeNode> GetCheckedNodes(this TreeView treeview) {
			var getNodes = LinqExtensions.Y<TreeNode, IEnumerable<TreeNode>>(f => n => new[] { n }.Concat((from TreeNode node in n.Nodes select node).SelectMany(f)));
			return (from TreeNode node in treeview.Nodes select node).SelectMany(getNodes).Where(n => n.Checked);
		}

		/// <summary>
		/// Finds a TreeNode amongst all TreeView nodes whose Tag equals the specified tag object.
		/// 
		/// If many nodes have tags matching the spec, only the first is returned.
		/// </summary>
		/// <param name="treeview"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
		public static TreeNode FindNodeByTag(this TreeView treeview, object tag) {
			if (tag == null)
				return null;

			var getNodes = LinqExtensions.Y<TreeNode, IEnumerable<TreeNode>>(f => n => new[] { n }.Concat((from TreeNode node in n.Nodes select node).SelectMany(f)));
			return (from TreeNode node in treeview.Nodes select node).SelectMany(getNodes).Where(n => tag.Equals(n.Tag)).FirstOrDefault();
		}

		/// <summary>
		/// Updates child nodes with the checked state of the parent.
		/// </summary>
		/// <param name="node"></param>
		public static void CheckChildnodes(this TreeNode node, bool isChecked) {
			foreach (TreeNode child in node.Nodes) {
				child.Checked = isChecked;
				CheckChildnodes(child, isChecked);
			}
		}

	}
}
