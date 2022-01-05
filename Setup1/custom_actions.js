// custom_actions.js <msi-file>
//
// Performs post-build fixups of our MSI.

// Constant values from Windows Installer:
var msiOpenDatabaseModeTransact = 1;

var msiViewModifyInsert = 1;
var msiViewModifyUpdate = 2;
var msiViewModifyAssign = 3;
var msiViewModifyReplace = 4;
var msiViewModifyDelete = 6;

var msidbCustomActionTypeInScript = 0x00000400;
var msidbCustomActionTypeNoImpersonate = 0x00000800;

if (WScript.Arguments.Length != 1) {
    WScript.StdErr.WriteLine(WScript.ScriptName + " file");
    WScript.Quit(1);
}

var filespec = WScript.Arguments(0);
var installer = WScript.CreateObject("WindowsInstaller.Installer");
var database = installer.OpenDatabase(filespec, msiOpenDatabaseModeTransact);

function MakeActionsNoImpersonate() {
    var sql = "SELECT `Action`, `Type`, `Source`, `Target` FROM `CustomAction`";
    var view = database.OpenView(sql);
    view.Execute();

    var record = view.Fetch();

    while (record) {
        if (record.IntegerData(2) & msidbCustomActionTypeInScript) {
            record.IntegerData(2) = record.IntegerData(2) | msidbCustomActionTypeNoImpersonate;
            view.Modify(msiViewModifyReplace, record);
        }

        record = view.Fetch();
    }

    view.Close();
}

function SetConditionOfAction(action, table, condition) {
    var sql = "SELECT `Action`, `Condition`, `Sequence` FROM " + table + " WHERE `Action`='" + action + "'";
    var view = database.OpenView(sql);
    view.Execute();

    var record = view.Fetch();

    record.StringData(2) = condition;
    view.Modify(msiViewModifyReplace, record);
    view.Close();
}

try {
    // Change all deferred custom actions to NoImpersonate:
    MakeActionsNoImpersonate();

    // Don't override AUTOUPDATE if it was specified on the command line:
    SetConditionOfAction("CustomCheckA_SetProperty_CHECKBOX1", "InstallExecuteSequence", "NOT(AUTOUPDATE)");
    SetConditionOfAction("CustomCheckA_SetProperty_CHECKBOX1", "InstallUISequence", "NOT(AUTOUPDATE)");

    database.Commit();
} catch (e) {
    WScript.StdErr.WriteLine(e);
    WScript.Quit(1);
}
