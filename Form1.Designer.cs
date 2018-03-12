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
            if (disposing && (components != null))
            {
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
            this.buttonUniques = new System.Windows.Forms.Button();
            this.listViewUniques = new System.Windows.Forms.ListView();
            this.columnUniqBaseType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnUniqSeverity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnUniqFilter = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnUniqExpect = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnUniqItems = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.listViewDiv = new System.Windows.Forms.ListView();
            this.columnDivName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDivSeverity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDivFilter = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDivExpect = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDivValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonDivCards = new System.Windows.Forms.Button();
            this.buttonGenFilter = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonUniques
            // 
            resources.ApplyResources(this.buttonUniques, "buttonUniques");
            this.buttonUniques.Name = "buttonUniques";
            this.buttonUniques.UseVisualStyleBackColor = true;
            this.buttonUniques.Click += new System.EventHandler(this.button1_Click);
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
            this.listViewUniques.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewUniques_ColumnClick);
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
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.listViewDiv);
            this.panel1.Controls.Add(this.listViewUniques);
            this.panel1.Name = "panel1";
            // 
            // listViewDiv
            // 
            this.listViewDiv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnDivName,
            this.columnDivSeverity,
            this.columnDivFilter,
            this.columnDivExpect,
            this.columnDivValue});
            resources.ApplyResources(this.listViewDiv, "listViewDiv");
            this.listViewDiv.GridLines = true;
            this.listViewDiv.Name = "listViewDiv";
            this.listViewDiv.UseCompatibleStateImageBehavior = false;
            this.listViewDiv.View = System.Windows.Forms.View.Details;
            this.listViewDiv.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewDiv_ColumnClick);
            this.listViewDiv.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_KeyUp);
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
            // buttonDivCards
            // 
            resources.ApplyResources(this.buttonDivCards, "buttonDivCards");
            this.buttonDivCards.Name = "buttonDivCards";
            this.buttonDivCards.UseVisualStyleBackColor = true;
            this.buttonDivCards.Click += new System.EventHandler(this.button2_Click);
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
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.buttonGenFilter);
            this.Controls.Add(this.buttonDivCards);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonUniques);
            this.Name = "Form1";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonUniques;
        private System.Windows.Forms.ListView listViewUniques;
        private System.Windows.Forms.ColumnHeader columnUniqBaseType;
        private System.Windows.Forms.ColumnHeader columnUniqFilter;
        private System.Windows.Forms.ColumnHeader columnUniqExpect;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ColumnHeader columnUniqSeverity;
        private System.Windows.Forms.Button buttonDivCards;
        private System.Windows.Forms.ListView listViewDiv;
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
    }
}

