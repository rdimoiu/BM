using System.Collections.Generic;

namespace BuildingManagement.Models
{
    public class TreeNode
    {
        public TreeNode()
        {
            children = new List<TreeNode>();
        }

        public string id { get; set; }

        public string text { get; set; }

        public List<TreeNode> children { get; set; }

        public TreeNodeState state { get; set; }
    }
}