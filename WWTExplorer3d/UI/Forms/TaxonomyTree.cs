using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace TerraViewer
{
    public partial class TaxonomyTree : Form
    {
        public TaxonomyTree()
        {
            InitializeComponent();
        }
        public string Taxonomy = "";

        void GetTaxonomy()
        {
            var taxonomy = new StringBuilder();
            var seperator = "";
            var scope = ((Scope)Scope.SelectedItem).ID + ".";

            foreach(TreeNode node in treeView1.Nodes)
            {
                if (node.Checked)
                {
                    taxonomy.Append(seperator + scope + node.Tag);
                    seperator = "; ";
                }

                foreach(TreeNode child in node.Nodes)
                {
                    if (child.Checked)
                    {
                        taxonomy.Append(seperator + scope + child.Tag);
                        seperator = "; ";
                    }
                    foreach(TreeNode grandChild in child.Nodes)
                    {
                        if (grandChild.Checked)
                        {
                            taxonomy.Append(seperator + scope + grandChild.Tag);
                            seperator = "; ";
                        }
                        foreach (TreeNode greatGrandChild in grandChild.Nodes)
                        {
                            if (greatGrandChild.Checked)
                            {
                                taxonomy.Append(seperator + scope + greatGrandChild.Tag);
                                seperator = "; ";
                            }
                        }
                    }
                }
            }
            Taxonomy = taxonomy.ToString();
        }
        void SetTaxonomy()
        {
            var idSplit = Taxonomy.Split(new[] {';'});
            var idList = new List<string>();
            if (idSplit.Length > 0 && !String.IsNullOrEmpty(idSplit[0]) )
            {
                Scope.SelectedIndex = idSplit[0].ToUpper()[0]-'A';

                for (var i = 0; i < idSplit.Length; i++)
		        {
                    try
                    {
			            idList.Add(idSplit[i].Trim().Substring(2));
                    }
                    catch
                    {
                        
                    }
		        }
            }

            foreach(TreeNode node in treeView1.Nodes)
            {
                node.Checked = false;
                for (var i = 0; i < idList.Count; i++)
		        {
                    if (idList[i] == (string)node.Tag)
                    {
                        idList.RemoveAt(i);
                        node.Checked = true;
                        break;
                    }
		        }


                foreach(TreeNode child in node.Nodes)
                {
                    child.Checked = false;
                    for (var i = 0; i < idList.Count; i++)
		            {
                        if (idList[i] == (string)child.Tag)
                        {
                            idList.RemoveAt(i);
                            child.Checked = true;
                            node.Expand();     
                            break;
                        }
		            }


                    foreach(TreeNode grandChild in child.Nodes)
                    {
                        grandChild.Checked = false;
                        for (var i = 0; i < idList.Count; i++)
		                {
    			            if (idList[i] == (string)grandChild.Tag)
                            {
                                idList.RemoveAt(i);
                                grandChild.Checked = true;
                                child.Expand();
                                node.Expand();          
                                break;
                            }
		                }

                        foreach (TreeNode greatGrandChild in grandChild.Nodes)
                        {
                            greatGrandChild.Checked = false;
                            for (var i = 0; i < idList.Count; i++)
                            {
                                if (idList[i] == (string)greatGrandChild.Tag)
                                {
                                    idList.RemoveAt(i);
                                    greatGrandChild.Checked = true;
                                    grandChild.Expand();
                                    child.Expand();
                                    node.Expand();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }


        private void TaxonomyTree_Load(object sender, EventArgs e)
        {
            Scope.Items.Add(new Scope(Language.GetLocalizedText(373, "Solar System"), "A"));
            Scope.Items.Add(new Scope(Language.GetLocalizedText(374, "Milky Way"), "B"));
            Scope.Items.Add(new Scope(Language.GetLocalizedText(375, "Local Universe"), "C"));
            Scope.Items.Add(new Scope(Language.GetLocalizedText(376, "Early Universe"), "D"));
            Scope.Items.Add(new Scope(Language.GetLocalizedText(377, "Unspecified"), "E"));
            Scope.Text = Language.GetLocalizedText(378, "Select a scope...");

            var Parents = new Stack<TreeNode>();
            Parents.Push(null);
            var filename = Properties.Settings.Default.CahceDirectory + string.Format(@"data\taxonomy_{0}.txt", Language.CurrentLanguage.Code);

            DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=taxonomy&L=" + Language.CurrentLanguage.Code, filename, false, false);
            var taxonomy = File.ReadAllText(filename, Encoding.Unicode);

            var sr = new StringReader(taxonomy);
           
            while( true)
            {
                var line = sr.ReadLine();
                if (line == null)
                {
                    break;
                }           
                
                var fields = line.Split(new[]{'\t'});
                if (fields.Length != 2)
                {
                    continue;
                }

                var id = fields[0];
                var name = fields[1];

                var node = new TreeNode(name);
                node.Tag = id;



                if (Parents.Peek() != null)
                {
                    var parent = Parents.Peek().Tag.ToString();
                    var len = parent.Length;
                    if (id.Length > len)
                    {
                        // Child Node of parent
                        Parents.Peek().Nodes.Add(node);
                        Parents.Push(node);
                    }
                    else if (id.Length == len)
                    {
                        // Sibling of parent
                        Parents.Pop();
                        Parents.Peek().Nodes.Add(node);
                        Parents.Push(node);
                    }
                    else
                    {
                        while (Parents.Peek() != null && Parents.Peek().Tag.ToString().Length >= id.Length) 
                        {
                            Parents.Pop();
                        }
                        if (Parents.Peek() != null)
                        {
                            Parents.Peek().Nodes.Add(node);
                            Parents.Push(node);
                        }
                        else
                        {
                            Parents.Push(node);
                            treeView1.Nodes.Add(node);
                        }
                    }
                }
                else
                {
                    Parents.Push(node);
                    treeView1.Nodes.Add(node);
                }
            }
            SetTaxonomy();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (Scope.SelectedIndex < 0)
            {
                return;
            }

            GetTaxonomy();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Scope_Load(object sender, EventArgs e)
        {

        }

        private void Scope_SelectionChanged(object sender, EventArgs e)
        {
            if (Scope.SelectedIndex != -1)
            {
                OK.Enabled = true;
            }
            else
            {
                OK.Enabled = false;
            }
        }

        private void treeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.ToString().Contains("["))
            {
                e.Cancel = true;
            }
        }
    }
    class Scope
    {
        public string Description;
        public string ID;
        public override string ToString()
        {
            return Description;
        }
        public Scope(string description, string id)
        {
            ID = id;
            Description = description;
        }
    }
}