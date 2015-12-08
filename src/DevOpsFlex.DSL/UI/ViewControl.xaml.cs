using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualStudio.Modeling;
using System.Linq;
using System.Collections.Generic;

namespace DevOpsFlex.DSL.UI
{
	/// <summary>
	/// Control enabling the visualization of the model
	/// </summary>
	public partial class ViewControl : UserControl
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ViewControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Strongly type access to the databound model
		/// </summary>
		DevOpsSystem Model
		{
			get
			{
				return this.DataContext as DevOpsSystem;
			}
		}

		#region User actions
		private void Delete_Execute(object sender, ExecutedRoutedEventArgs e)
		{
			DeleteSelectedModelElements();
		}

		private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			DeleteSelectedModelElements();
		}

		private void AddButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Model.DevOpsAlerted.AddNew();
		}


		/// <summary>
		/// Deletes the selected model elements
		/// </summary>
		private void DeleteSelectedModelElements()
		{
			// Any modification to the model must be done in the context of a modeling transaction
			using (Transaction t = this.Model.Store.TransactionManager.BeginTransaction("deleting elements"))
			{
				// The last row of the DataGrid is NOT a ModelElement, and the user can select it
				// we must only take into account the ModelElements
				foreach (ModelElement m in elementsGrid.SelectedItems.OfType<ModelElement>())
				{
					m.Delete();
				}

				// Only commit the transaction if something was indded changeed
				if (t.HasPendingChanges)
				{
					t.Commit();
				}
			}
		}
		#endregion

	}
}
