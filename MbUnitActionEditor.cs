using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web.Controls;
using Inedo.BuildMaster.Web.Controls.Extensions;
using Inedo.Web.Controls;

namespace Inedo.BuildMasterExtensions.MbUnit
{
    internal sealed class MbUnitActionEditor : ActionEditorBase
    {
        private ValidatingTextBox txtExePath;
        private ValidatingTextBox txtTestFile;
        private ValidatingTextBox txtGroupName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MbUnitActionEditor"/> class.
        /// </summary>
        public MbUnitActionEditor()
        {
        }

        public override bool DisplaySourceDirectory
        {
            get { return true; }
        }

        public override void BindToForm(ActionBase extension)
        {
            this.EnsureChildControls();

            var action = (MbUnitAppAction)extension;
            this.txtExePath.Text = action.ExePath;
            this.txtTestFile.Text = action.TestFile;
            this.txtGroupName.Text = action.GroupName;
        }
        public override ActionBase CreateFromForm()
        {
            this.EnsureChildControls();

            return new MbUnitAppAction
            {
                ExePath = this.txtExePath.Text,
                TestFile = this.txtTestFile.Text,
                GroupName = this.txtGroupName.Text
            };
        }

        protected override void CreateChildControls()
        {
            this.txtExePath = new ValidatingTextBox
            {
                Required = true,
                Width = 300
            };

            this.txtTestFile = new ValidatingTextBox
            {
                Required = true,
                Width = 300
            };

            this.txtGroupName = new ValidatingTextBox
            {
                Required = true,
                Width = 300
            };

            this.Controls.Add(
                new FormFieldGroup(
                    "MBUnit Location",
                    "Specify the full path (including the file name) of MBUnit.exe",
                    false,
                    new StandardFormField(
                        "MBUnit.exe Path:",
                        this.txtExePath
                    )
                ),
                new FormFieldGroup(
                    "File to Test",
                    "Specify the file or project to test.",
                    false,
                    new StandardFormField(
                        "File to Test:",
                        this.txtTestFile
                    )
                ),
                new FormFieldGroup(
                    "Group Name",
                    "Optionally provide a Unit Test Group name for these tests.",
                    true,
                    new StandardFormField(
                        "Group Name:",
                        this.txtGroupName
                    )
                )
            );
        }
    }
}
