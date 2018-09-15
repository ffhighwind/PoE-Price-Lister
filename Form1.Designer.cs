namespace PoE_Price_Lister
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.listViewUniques = new System.Windows.Forms.ListView();
            this.columnUniqBaseType = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnUniqSeverity = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnUniqFilter = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnUniqExpect = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnUniqItems = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.listViewDivination = new System.Windows.Forms.ListView();
            this.columnDivName = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnDivSeverity = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnDivFilter = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnDivExpect = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnDivValue = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.buttonGenFilter = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabUniquesSC = new System.Windows.Forms.TabPage();
            this.tabDivinationSC = new System.Windows.Forms.TabPage();
            this.tabUniquesHC = new System.Windows.Forms.TabPage();
            this.listViewUniquesHC = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.tabDivinationHC = new System.Windows.Forms.TabPage();
            this.listViewDivinationHC = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabUniquesSC.SuspendLayout();
            this.tabDivinationSC.SuspendLayout();
            this.tabUniquesHC.SuspendLayout();
            this.tabDivinationHC.SuspendLayout();
            this.SuspendLayout();
            //
            // listViewUniques
            //
            this.listViewUniques.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnUniqBaseType,
            this.columnUniqSeverity,
            this.columnUniqFilter,
            this.columnUniqExpect,
            this.columnUniqItems});
            resources.ApplyResources(this.listViewUniques, "listViewUniques");
            this.listViewUniques.GridLines = true;
            this.listViewUniques.Name = "listViewUniques";
            this.listViewUniques.UseCompatibleStateImageBehavior = false;
            this.listViewUniques.View = System.Windows.Forms.View.Details;
            this.listViewUniques.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            this.listViewUniques.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_KeyUp);
            //
            // columnUniqBaseType
            //
            resources.ApplyResources(this.columnUniqBaseType, "columnUniqBaseType");
            //
            // columnUniqSeverity
            //
            resources.ApplyResources(this.columnUniqSeverity, "columnUniqSeverity");
            //
            // columnUniqFilter
            //
            resources.ApplyResources(this.columnUniqFilter, "columnUniqFilter");
            //
            // columnUniqExpect
            //
            resources.ApplyResources(this.columnUniqExpect, "columnUniqExpect");
            //
            // columnUniqItems
            //
            resources.ApplyResources(this.columnUniqItems, "columnUniqItems");
            //
            // listViewDivination
            //
            this.listViewDivination.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnDivName,
            this.columnDivSeverity,
            this.columnDivFilter,
            this.columnDivExpect,
            this.columnDivValue});
            resources.ApplyResources(this.listViewDivination, "listViewDivination");
            this.listViewDivination.GridLines = true;
            this.listViewDivination.Name = "listViewDivination";
            this.listViewDivination.UseCompatibleStateImageBehavior = false;
            this.listViewDivination.View = System.Windows.Forms.View.Details;
            this.listViewDivination.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            this.listViewDivination.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_KeyUp);
            //
            // columnDivName
            //
            resources.ApplyResources(this.columnDivName, "columnDivName");
            //
            // columnDivSeverity
            //
            resources.ApplyResources(this.columnDivSeverity, "columnDivSeverity");
            //
            // columnDivFilter
            //
            resources.ApplyResources(this.columnDivFilter, "columnDivFilter");
            //
            // columnDivExpect
            //
            resources.ApplyResources(this.columnDivExpect, "columnDivExpect");
            //
            // columnDivValue
            //
            resources.ApplyResources(this.columnDivValue, "columnDivValue");
            //
            // buttonGenFilter
            //
            resources.ApplyResources(this.buttonGenFilter, "buttonGenFilter");
            this.buttonGenFilter.Name = "buttonGenFilter";
            this.buttonGenFilter.UseVisualStyleBackColor = true;
            this.buttonGenFilter.Click += new System.EventHandler(this.buttonGenFilter_Click);
            //
            // checkBox1
            //
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            //
            // buttonLoad
            //
            resources.ApplyResources(this.buttonLoad, "buttonLoad");
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            //
            // openFileDialog1
            //
            this.openFileDialog1.FileName = "openFileDialog1";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            this.openFileDialog1.RestoreDirectory = true;
            //
            // tabControl1
            //
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabUniquesSC);
            this.tabControl1.Controls.Add(this.tabDivinationSC);
            this.tabControl1.Controls.Add(this.tabUniquesHC);
            this.tabControl1.Controls.Add(this.tabDivinationHC);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            //
            // tabUniquesSC
            //
            this.tabUniquesSC.Controls.Add(this.listViewUniques);
            resources.ApplyResources(this.tabUniquesSC, "tabUniquesSC");
            this.tabUniquesSC.Name = "tabUniquesSC";
            this.tabUniquesSC.UseVisualStyleBackColor = true;
            //
            // tabDivinationSC
            //
            this.tabDivinationSC.Controls.Add(this.listViewDivination);
            resources.ApplyResources(this.tabDivinationSC, "tabDivinationSC");
            this.tabDivinationSC.Name = "tabDivinationSC";
            this.tabDivinationSC.UseVisualStyleBackColor = true;
            //
            // tabUniquesHC
            //
            this.tabUniquesHC.Controls.Add(this.listViewUniquesHC);
            resources.ApplyResources(this.tabUniquesHC, "tabUniquesHC");
            this.tabUniquesHC.Name = "tabUniquesHC";
            this.tabUniquesHC.UseVisualStyleBackColor = true;
            //
            // listViewUniquesHC
            //
            this.listViewUniquesHC.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            resources.ApplyResources(this.listViewUniquesHC, "listViewUniquesHC");
            this.listViewUniquesHC.GridLines = true;
            this.listViewUniquesHC.Name = "listViewUniquesHC";
            this.listViewUniquesHC.UseCompatibleStateImageBehavior = false;
            this.listViewUniquesHC.View = System.Windows.Forms.View.Details;
            this.listViewUniquesHC.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            this.listViewUniquesHC.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_KeyUp);
            //
            // columnHeader1
            //
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            //
            // columnHeader2
            //
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            //
            // columnHeader3
            //
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            //
            // columnHeader4
            //
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            //
            // columnHeader5
            //
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            //
            // tabDivinationHC
            //
            this.tabDivinationHC.Controls.Add(this.listViewDivinationHC);
            resources.ApplyResources(this.tabDivinationHC, "tabDivinationHC");
            this.tabDivinationHC.Name = "tabDivinationHC";
            this.tabDivinationHC.UseVisualStyleBackColor = true;
            //
            // listViewDivinationHC
            //
            this.listViewDivinationHC.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10});
            resources.ApplyResources(this.listViewDivinationHC, "listViewDivinationHC");
            this.listViewDivinationHC.GridLines = true;
            this.listViewDivinationHC.Name = "listViewDivinationHC";
            this.listViewDivinationHC.UseCompatibleStateImageBehavior = false;
            this.listViewDivinationHC.View = System.Windows.Forms.View.Details;
            this.listViewDivinationHC.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            this.listViewDivinationHC.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_KeyUp);
            //
            // columnHeader6
            //
            resources.ApplyResources(this.columnHeader6, "columnHeader6");
            //
            // columnHeader7
            //
            resources.ApplyResources(this.columnHeader7, "columnHeader7");
            //
            // columnHeader8
            //
            resources.ApplyResources(this.columnHeader8, "columnHeader8");
            //
            // columnHeader9
            //
            resources.ApplyResources(this.columnHeader9, "columnHeader9");
            //
            // columnHeader10
            //
            resources.ApplyResources(this.columnHeader10, "columnHeader10");
            //
            // checkBox2
            //
            resources.ApplyResources(this.checkBox2, "checkBox2");
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.UseVisualStyleBackColor = true;
            //
            // Form1
            //
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.buttonGenFilter);
            this.Name = "Form1";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.tabControl1.ResumeLayout(false);
            this.tabUniquesSC.ResumeLayout(false);
            this.tabDivinationSC.ResumeLayout(false);
            this.tabUniquesHC.ResumeLayout(false);
            this.tabDivinationHC.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView listViewUniques;
        private System.Windows.Forms.ColumnHeader columnUniqBaseType;
        private System.Windows.Forms.ColumnHeader columnUniqFilter;
        private System.Windows.Forms.ColumnHeader columnUniqExpect;
        private System.Windows.Forms.ColumnHeader columnUniqSeverity;
        private System.Windows.Forms.ListView listViewDivination;
        private System.Windows.Forms.ColumnHeader columnDivName;
        private System.Windows.Forms.ColumnHeader columnDivFilter;
        private System.Windows.Forms.ColumnHeader columnDivExpect;
        private System.Windows.Forms.ColumnHeader columnDivValue;
        private System.Windows.Forms.ColumnHeader columnUniqItems;
        private System.Windows.Forms.ColumnHeader columnDivSeverity;
        private System.Windows.Forms.Button buttonGenFilter;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabUniquesSC;
        private System.Windows.Forms.TabPage tabDivinationSC;
        private System.Windows.Forms.TabPage tabUniquesHC;
        private System.Windows.Forms.TabPage tabDivinationHC;
        private System.Windows.Forms.ListView listViewUniquesHC;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ListView listViewDivinationHC;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}

