﻿using System;
using System.Linq;
using System.Windows.Forms;
using KnowledgeBase.Conditions;

namespace GAIPS.AssetEditorTools
{
	public partial class ConditionSetEditorControl : UserControl
	{
		private ConditionSetView _view;

		public ConditionSetView View
		{
			get { return _view; }
			set
			{
				UnbindView(_view);
				_view = value; 
				BindView(_view);
				UpdateControl();
			}
		}

		private int CurrentSelectedRowIndex
		{
			get
			{
				if (dataView.CurrentCell!=null && dataView.CurrentCell.Selected)
					return dataView.CurrentCell.RowIndex;

				if (dataView.SelectedRows.Count == 0)
					return -1;

				return dataView.SelectedRows[0].Index;
			}
		}

		private string CurrentSelectedValue {
			get { return (string)dataView.Rows[CurrentSelectedRowIndex].Cells[0].Value; }
			set { dataView.Rows[CurrentSelectedRowIndex].Cells[0].Value = value; }
		}

		public ConditionSetEditorControl()
		{
			InitializeComponent();

			quantifierComboBox.DataSource = ConditionSetView.QuantifierTypes;
			UpdateControl();
		}

		private void UnbindView(ConditionSetView view)
		{
			if(view==null)
				return;

			view.OnRefresh -= UpdateControl;
			dataView.DataSource = null;
		}

		private void BindView(ConditionSetView view)
		{
			if (view == null)
				return;

			view.OnRefresh += UpdateControl;
			dataView.DataSource = view.Conditions;
			foreach (var c in dataView.Columns.Cast<DataGridViewColumn>())
			{
				c.SortMode = DataGridViewColumnSortMode.NotSortable;
			}
		}

		private void UpdateControl()
		{
			var hasData = _view != null && _view.HasData;
			var canEdit = hasData && _view.Conditions.Count > 0;

			updateMoveButtonStatus(hasData);
			add_button.Enabled = hasData;
			remove_button.Enabled = canEdit;
			quantifierComboBox.Enabled = hasData;
		}

		private void updateMoveButtonStatus(bool hasData)
		{

			moveDown_button.Enabled = hasData && CurrentSelectedRowIndex>=0 && CurrentSelectedRowIndex < (dataView.RowCount - 1);
			moveUp_button.Enabled = hasData && CurrentSelectedRowIndex >= 0 && CurrentSelectedRowIndex > 0; ;
		}

		private void add_button_Click(object sender, EventArgs e)
		{
			var hasData = _view != null && _view.HasData;
			if(!hasData)
				return;

			_view.AddÑewDefaultCondition();
		}

		private void remove_button_Click(object sender, EventArgs e)
		{
			var hasData = _view != null && _view.HasData;
			if (!hasData)
				return;

			_view.RemoveConditionAt(CurrentSelectedRowIndex);
		}

		private string _savedValue;

		private void dataView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			_savedValue = CurrentSelectedValue;
		}

		private void dataView_CellEndEdit(object sender, DataGridViewCellEventArgs evt)
		{
			try
			{
				var newValue = CurrentSelectedValue;

				if (string.Equals(newValue, _savedValue, StringComparison.InvariantCultureIgnoreCase))
					return;

				if (string.IsNullOrWhiteSpace(newValue))
				{
					_view.RemoveConditionAt(CurrentSelectedRowIndex);
					return;
				}

				try
				{
					var c = Condition.Parse(newValue);
					_view.ChangeConditionAtIndex(CurrentSelectedRowIndex,newValue);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					CurrentSelectedValue = _savedValue;
				}
			}
			finally
			{
				_savedValue = null;
			}
		}

		private void quantifierComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			var hasData = _view != null && _view.HasData;
			if (!hasData)
				return;

			_view.SetQuantifier((LogicalQuantifier)quantifierComboBox.SelectedItem);
		}

		private void dataView_SelectionChanged(object sender, EventArgs e)
		{
			updateMoveButtonStatus(_view != null && _view.HasData);
		}

		private void moveUp_button_Click(object sender, EventArgs e)
		{
			var hasData = _view != null && _view.HasData;
			if (!hasData)
				return;

			var index = CurrentSelectedRowIndex;
			_view.MoveCondition(index,-1);
			dataView.ClearSelection();

			dataView.Rows[index - 1].Selected = true;
			updateMoveButtonStatus(true);
		}

		private void moveDown_button_Click(object sender, EventArgs e)
		{
			var hasData = _view != null && _view.HasData;
			if (!hasData)
				return;

			var index = CurrentSelectedRowIndex;
			_view.MoveCondition(index, 1);
			dataView.ClearSelection();

			dataView.Rows[index + 1].Selected=true;
			updateMoveButtonStatus(true);
		}
	}
}