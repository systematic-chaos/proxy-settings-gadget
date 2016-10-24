var obj = document.getElementById('ProxyActiveX');

// Initialize the gadget
function initializeProxyWidget() {
  try {
      if (obj) {
          updateProxyEnabledCheckbox();
      } else {
          alert('ActiveX Control object is not created!');
      }
  } catch(ex) {
      alert('Some error happens, error message is: ' + ex.message);
  }
};

var address = '', exceptions = '', authenticate = false, user = '', pass = '';

function enableProxy(checked) {
    if (checked) {
        if (address != '') {
            if (authenticate && user != '' && pass != '')
                obj.EnableProxyWithAuthentication(address, exceptions,
                     user, pass);
            else
                obj.EnableProxy(address, exceptions);
        } else {
            // Programmatically raise the OnShowSettings event
            /*document.getElementById('cboxEnableProxy')
                .fireEvent('System.Gadget.onShowSettings');*/
        }  
    } else {
      obj.DisableProxy();
    }
    updateProxyEnabledCheckbox();
}

function updateProxyEnabledCheckbox() {
    document.getElementById('cboxEnableProxy').checked = obj.IsProxyEnabled();
}

System.Gadget.onSettingsClosed = SettingsClosed;

// ----------------------------------------------------------------------------
// Handle the Settings dialog closed event.
// event - System.Gadget.Settings.onSettingsClosed event argument
// ----------------------------------------------------------------------------
function SettingsClosed(event) {
    // User hits OK on the settings page
    if (event.closeAction == event.Action.commit) {
        address = System.Gadget.Settings.readString('proxyAddress');
        exceptions = System.Gadget.Settings.readString('proxyExceptions');
        authenticate = System.Gadget.Settings.read('authentication');
        if (authenticate) {
            user = System.Gadget.Settings.readString('proxyUser');
            pass = System.Gadget.Settings.readString('proxyPassword');
            if (pass != '') {
              pass = obj.Md5Digest(pass);
            }
        }
    }
    // User hits Cancel on the settings page
    else if (event.closeAction == event.Action.cancel);
}
