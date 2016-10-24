function authenticate(checked) {
  document.getElementById('user').disabled = !checked;
  document.getElementById('pass').disabled = !checked; 
}

// Delegate for the show settings event
System.Gadget.onShowSettings = ShowSettings;

// Delegate for the settings closing event
System.Gadget.onSettingsClosing = SettingsClosing;

// ----------------------------------------------------------------------------
// Handle the Settings dialog show event.
// Parameters:
// event - System.Gadget.onShowSettings event argument
// ----------------------------------------------------------------------------
function ShowSettings(event) {
    document.getElementById('address').value
         = System.Gadget.Settings.readString('proxyAddress');
    document.getElementById('exceptions').value
         = System.Gadget.Settings.readString('proxyExceptions');
    authenticate(document.getElementById('authentication').checked
         = System.Gadget.Settings.read('authentication')
              && System.Gadget.Settings.read('authentication') != '');
    document.getElementById('user').value
         = System.Gadget.Settings.readString('proxyUser');
    document.getElementById('pass').value
         = System.Gadget.Settings.readString('proxyPassword');
}

// ----------------------------------------------------------------------------
// Handle the Settings dialog closing event.
// Parameters:
// event - System.Gadget.onSettingsClosing event argument
// ----------------------------------------------------------------------------
function SettingsClosing(event) {
    // Save the settings if the user clicked OK
    if (event.closeAction == event.Action.commit) {
        System.Gadget.Settings.writeString('proxyAddress',
             document.getElementById('address').value);
        System.Gadget.Settings.writeString('proxyExceptions',
             document.getElementById('exceptions').value);
        var authenticate = document.getElementById('authentication').checked;
        System.Gadget.Settings.write('authentication', authenticate);
        if (authenticate) {
            System.Gadget.Settings.writeString('proxyUser',
                 document.getElementById('user').value);
            System.Gadget.Settings.writeString('proxyPassword',
                 document.getElementById('pass').value);
        }
    }
    // Allow the Settings dialog to close
    event.cancel = false;
}