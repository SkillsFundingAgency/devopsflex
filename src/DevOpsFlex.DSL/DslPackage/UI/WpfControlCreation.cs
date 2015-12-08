// --------------------------------------
// This is NOT an auto-generated file.
// --------------------------------------
namespace DevOpsFlex.DSL
{
	using global::System.Diagnostics;
	using global::System.Windows.Controls;

	/// <summary>
	/// Partial implementation of the class generated with the DocView.tt
	/// Its Intended to be fully customized i.e. Change the control, databinding scope and selection monitoring.
	/// </summary>
	partial class DevOpsFlexDocView
	{
		/// <summary>
		/// An instance of the view control that visualizes the model
		/// </summary>
		private DevOpsFlex.DSL.UI.ViewControl viewControl = null;

		/// <summary>
		/// This methods creates the WPF control that will represent the model
		/// </summary>
		protected override global::System.Windows.UIElement CreateControl()
		{
			this.viewControl = new DevOpsFlex.DSL.UI.ViewControl();
			return this.viewControl;
		}

		/// <summary>
		/// Connects the RootElement to the view and starts monitoring the selection
		/// </summary>
		protected override void ConnectToModel()
		{
			Debug.Assert(viewControl!=null);
			if (viewControl != null)
			{
				// Bind to the model
				viewControl.DataContext = this.DocData.RootElement;
				// Monitor the selection
				this.SelectionPusher.Add(viewControl);
			}
		}

		/// <summary>
		/// Disconnects the view from the model and stops monitoring for selection
		/// </summary>
		protected override void DisconnectFromModel()
		{
			Debug.Assert(viewControl!=null);
			if (viewControl != null)
			{
				// Stop monitoring the selection
				this.SelectionPusher.Remove(viewControl);
				// At this point we can't change the model since we passed the point in which 
				// the model will be persisted (and in case of change there will be a save dialog as well).
				// So, we just need to ensure clear the databinding so that there won't be any attempts 
				// to try and read from the model or write to it, which potentially can result in exception.
				viewControl.DataContext = null;
			}
		}
	}
}
