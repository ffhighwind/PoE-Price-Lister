namespace PoE_Price_Lister
{
    partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.buttonGenFilter = new System.Windows.Forms.Button();
			this.buttonLoad = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.enchantsSCTab = new System.Windows.Forms.TabControl();
			this.tabUniquesSC = new System.Windows.Forms.TabPage();
			this.uniquesScDataGridView = new System.Windows.Forms.DataGridView();
			this.tabDivinationSC = new System.Windows.Forms.TabPage();
			this.divinationScDataGridView = new System.Windows.Forms.DataGridView();
			this.tabEnchantsSC = new System.Windows.Forms.TabPage();
			this.enchantsScDataGridView = new System.Windows.Forms.DataGridView();
			this.tabUniquesHC = new System.Windows.Forms.TabPage();
			this.uniquesHcDataGridView = new System.Windows.Forms.DataGridView();
			this.tabDivinationHC = new System.Windows.Forms.TabPage();
			this.divinationHcDataGridView = new System.Windows.Forms.DataGridView();
			this.tabEnchantsHC = new System.Windows.Forms.TabPage();
			this.enchantsHcDataGridView = new System.Windows.Forms.DataGridView();
			this.scRadioButton = new System.Windows.Forms.RadioButton();
			this.hcRadioButton = new System.Windows.Forms.RadioButton();
			this.hcFriendlyRadioButton = new System.Windows.Forms.RadioButton();
			this.enchantsSCTab.SuspendLayout();
			this.tabUniquesSC.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.uniquesScDataGridView)).BeginInit();
			this.tabDivinationSC.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.divinationScDataGridView)).BeginInit();
			this.tabEnchantsSC.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.enchantsScDataGridView)).BeginInit();
			this.tabUniquesHC.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.uniquesHcDataGridView)).BeginInit();
			this.tabDivinationHC.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.divinationHcDataGridView)).BeginInit();
			this.tabEnchantsHC.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.enchantsHcDataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonGenFilter
			// 
			resources.ApplyResources(this.buttonGenFilter, "buttonGenFilter");
			this.buttonGenFilter.Name = "buttonGenFilter";
			this.buttonGenFilter.UseVisualStyleBackColor = true;
			this.buttonGenFilter.Click += new System.EventHandler(this.buttonGenFilter_Click);
			// 
			// buttonLoad
			// 
			resources.ApplyResources(this.buttonLoad, "buttonLoad");
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.UseVisualStyleBackColor = true;
			this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.FileName = "openFileDialog";
			resources.ApplyResources(this.openFileDialog, "openFileDialog");
			this.openFileDialog.RestoreDirectory = true;
			// 
			// enchantsSCTab
			// 
			resources.ApplyResources(this.enchantsSCTab, "enchantsSCTab");
			this.enchantsSCTab.Controls.Add(this.tabUniquesSC);
			this.enchantsSCTab.Controls.Add(this.tabDivinationSC);
			this.enchantsSCTab.Controls.Add(this.tabEnchantsSC);
			this.enchantsSCTab.Controls.Add(this.tabUniquesHC);
			this.enchantsSCTab.Controls.Add(this.tabDivinationHC);
			this.enchantsSCTab.Controls.Add(this.tabEnchantsHC);
			this.enchantsSCTab.Name = "enchantsSCTab";
			this.enchantsSCTab.SelectedIndex = 0;
			// 
			// tabUniquesSC
			// 
			this.tabUniquesSC.Controls.Add(this.uniquesScDataGridView);
			resources.ApplyResources(this.tabUniquesSC, "tabUniquesSC");
			this.tabUniquesSC.Name = "tabUniquesSC";
			this.tabUniquesSC.UseVisualStyleBackColor = true;
			// 
			// uniquesScDataGridView
			// 
			this.uniquesScDataGridView.AllowUserToAddRows = false;
			this.uniquesScDataGridView.AllowUserToDeleteRows = false;
			this.uniquesScDataGridView.AllowUserToResizeRows = false;
			this.uniquesScDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.uniquesScDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.uniquesScDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.uniquesScDataGridView, "uniquesScDataGridView");
			this.uniquesScDataGridView.Name = "uniquesScDataGridView";
			this.uniquesScDataGridView.ReadOnly = true;
			this.uniquesScDataGridView.RowHeadersVisible = false;
			// 
			// tabDivinationSC
			// 
			this.tabDivinationSC.Controls.Add(this.divinationScDataGridView);
			resources.ApplyResources(this.tabDivinationSC, "tabDivinationSC");
			this.tabDivinationSC.Name = "tabDivinationSC";
			this.tabDivinationSC.UseVisualStyleBackColor = true;
			// 
			// divinationScDataGridView
			// 
			this.divinationScDataGridView.AllowUserToAddRows = false;
			this.divinationScDataGridView.AllowUserToDeleteRows = false;
			this.divinationScDataGridView.AllowUserToResizeRows = false;
			this.divinationScDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.divinationScDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.divinationScDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.divinationScDataGridView, "divinationScDataGridView");
			this.divinationScDataGridView.Name = "divinationScDataGridView";
			this.divinationScDataGridView.ReadOnly = true;
			this.divinationScDataGridView.RowHeadersVisible = false;
			// 
			// tabEnchantsSC
			// 
			this.tabEnchantsSC.Controls.Add(this.enchantsScDataGridView);
			resources.ApplyResources(this.tabEnchantsSC, "tabEnchantsSC");
			this.tabEnchantsSC.Name = "tabEnchantsSC";
			this.tabEnchantsSC.UseVisualStyleBackColor = true;
			// 
			// enchantsScDataGridView
			// 
			this.enchantsScDataGridView.AllowUserToAddRows = false;
			this.enchantsScDataGridView.AllowUserToDeleteRows = false;
			this.enchantsScDataGridView.AllowUserToResizeRows = false;
			this.enchantsScDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.enchantsScDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.enchantsScDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.enchantsScDataGridView, "enchantsScDataGridView");
			this.enchantsScDataGridView.Name = "enchantsScDataGridView";
			this.enchantsScDataGridView.ReadOnly = true;
			this.enchantsScDataGridView.RowHeadersVisible = false;
			// 
			// tabUniquesHC
			// 
			this.tabUniquesHC.Controls.Add(this.uniquesHcDataGridView);
			resources.ApplyResources(this.tabUniquesHC, "tabUniquesHC");
			this.tabUniquesHC.Name = "tabUniquesHC";
			this.tabUniquesHC.UseVisualStyleBackColor = true;
			// 
			// uniquesHcDataGridView
			// 
			this.uniquesHcDataGridView.AllowUserToAddRows = false;
			this.uniquesHcDataGridView.AllowUserToDeleteRows = false;
			this.uniquesHcDataGridView.AllowUserToResizeRows = false;
			this.uniquesHcDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.uniquesHcDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.uniquesHcDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.uniquesHcDataGridView, "uniquesHcDataGridView");
			this.uniquesHcDataGridView.Name = "uniquesHcDataGridView";
			this.uniquesHcDataGridView.ReadOnly = true;
			this.uniquesHcDataGridView.RowHeadersVisible = false;
			// 
			// tabDivinationHC
			// 
			this.tabDivinationHC.Controls.Add(this.divinationHcDataGridView);
			resources.ApplyResources(this.tabDivinationHC, "tabDivinationHC");
			this.tabDivinationHC.Name = "tabDivinationHC";
			this.tabDivinationHC.UseVisualStyleBackColor = true;
			// 
			// divinationHcDataGridView
			// 
			this.divinationHcDataGridView.AllowUserToAddRows = false;
			this.divinationHcDataGridView.AllowUserToDeleteRows = false;
			this.divinationHcDataGridView.AllowUserToResizeRows = false;
			this.divinationHcDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.divinationHcDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.divinationHcDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.divinationHcDataGridView, "divinationHcDataGridView");
			this.divinationHcDataGridView.Name = "divinationHcDataGridView";
			this.divinationHcDataGridView.ReadOnly = true;
			this.divinationHcDataGridView.RowHeadersVisible = false;
			// 
			// tabEnchantsHC
			// 
			this.tabEnchantsHC.Controls.Add(this.enchantsHcDataGridView);
			resources.ApplyResources(this.tabEnchantsHC, "tabEnchantsHC");
			this.tabEnchantsHC.Name = "tabEnchantsHC";
			this.tabEnchantsHC.UseVisualStyleBackColor = true;
			// 
			// enchantsHcDataGridView
			// 
			this.enchantsHcDataGridView.AllowUserToAddRows = false;
			this.enchantsHcDataGridView.AllowUserToDeleteRows = false;
			this.enchantsHcDataGridView.AllowUserToResizeRows = false;
			this.enchantsHcDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.enchantsHcDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.enchantsHcDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.enchantsHcDataGridView, "enchantsHcDataGridView");
			this.enchantsHcDataGridView.Name = "enchantsHcDataGridView";
			this.enchantsHcDataGridView.ReadOnly = true;
			this.enchantsHcDataGridView.RowHeadersVisible = false;
			// 
			// scRadioButton
			// 
			resources.ApplyResources(this.scRadioButton, "scRadioButton");
			this.scRadioButton.Name = "scRadioButton";
			this.scRadioButton.UseVisualStyleBackColor = true;
			this.scRadioButton.CheckedChanged += new System.EventHandler(this.scRadioButton_CheckedChanged);
			// 
			// hcRadioButton
			// 
			resources.ApplyResources(this.hcRadioButton, "hcRadioButton");
			this.hcRadioButton.Name = "hcRadioButton";
			this.hcRadioButton.UseVisualStyleBackColor = true;
			this.hcRadioButton.CheckedChanged += new System.EventHandler(this.hcRadioButton_CheckedChanged);
			// 
			// hcFriendlyRadioButton
			// 
			resources.ApplyResources(this.hcFriendlyRadioButton, "hcFriendlyRadioButton");
			this.hcFriendlyRadioButton.Checked = true;
			this.hcFriendlyRadioButton.Name = "hcFriendlyRadioButton";
			this.hcFriendlyRadioButton.TabStop = true;
			this.hcFriendlyRadioButton.UseVisualStyleBackColor = true;
			this.hcFriendlyRadioButton.CheckedChanged += new System.EventHandler(this.hcFriendlyRadioButton_CheckedChanged);
			// 
			// MainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.hcFriendlyRadioButton);
			this.Controls.Add(this.hcRadioButton);
			this.Controls.Add(this.scRadioButton);
			this.Controls.Add(this.enchantsSCTab);
			this.Controls.Add(this.buttonLoad);
			this.Controls.Add(this.buttonGenFilter);
			this.Name = "MainForm";
			this.Shown += new System.EventHandler(this.Form1_Shown);
			this.enchantsSCTab.ResumeLayout(false);
			this.tabUniquesSC.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.uniquesScDataGridView)).EndInit();
			this.tabDivinationSC.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.divinationScDataGridView)).EndInit();
			this.tabEnchantsSC.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.enchantsScDataGridView)).EndInit();
			this.tabUniquesHC.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.uniquesHcDataGridView)).EndInit();
			this.tabDivinationHC.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.divinationHcDataGridView)).EndInit();
			this.tabEnchantsHC.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.enchantsHcDataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonGenFilter;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TabControl enchantsSCTab;
        private System.Windows.Forms.TabPage tabUniquesSC;
        private System.Windows.Forms.TabPage tabDivinationSC;
        private System.Windows.Forms.TabPage tabUniquesHC;
        private System.Windows.Forms.TabPage tabDivinationHC;
		private System.Windows.Forms.TabPage tabEnchantsSC;
		private System.Windows.Forms.TabPage tabEnchantsHC;
		private System.Windows.Forms.DataGridView uniquesScDataGridView;
		private System.Windows.Forms.DataGridView divinationScDataGridView;
		private System.Windows.Forms.DataGridView uniquesHcDataGridView;
		private System.Windows.Forms.DataGridView divinationHcDataGridView;
		private System.Windows.Forms.DataGridView enchantsScDataGridView;
		private System.Windows.Forms.DataGridView enchantsHcDataGridView;
		private System.Windows.Forms.RadioButton scRadioButton;
		private System.Windows.Forms.RadioButton hcRadioButton;
		private System.Windows.Forms.RadioButton hcFriendlyRadioButton;
	}
}

