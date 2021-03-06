using System;
using System.Linq;
using UIKit;


namespace Acr.MvvmCross.Plugins.UserDialogs.Touch {

    public class TouchUserDialogService : AbstractTouchUserDialogService {

        public override void ActionSheet(ActionSheetConfig config) {
            this.Dispatch(() =>  {
                var action = new UIActionSheet(config.Title);
                config.Options.ToList().ForEach(x => action.AddButton(x.Text));

                action.Dismissed += (sender, btn) => {
                    if (btn.ButtonIndex > -1 && btn.ButtonIndex < config.Options.Count - 1)
						config.Options[(int)btn.ButtonIndex].Action();
                };
                var view = Utils.GetTopView();
                action.ShowInView(view);
            });
        }


        public override void Alert(AlertConfig config) {
            this.Dispatch(() =>  {
                var dlg = new UIAlertView(config.Title ?? String.Empty, config.Message, null, null, config.OkText);
                if (config.OnOk != null)
                    dlg.Clicked += (s, e) => config.OnOk();

                dlg.Show();
            });
        }


        public override void Confirm(ConfirmConfig config) {
            this.Dispatch(() =>  {
                var dlg = new UIAlertView(config.Title ?? String.Empty, config.Message, null, config.CancelText, config.OkText);
                dlg.Clicked += (s, e) => {
                    var ok = (dlg.CancelButtonIndex != e.ButtonIndex);
                    config.OnConfirm(ok);
                };
                dlg.Show();
            });
        }


        public override void Login(LoginConfig config) {
            this.Dispatch(() => {
                var dlg = new UIAlertView { AlertViewStyle = UIAlertViewStyle.LoginAndPasswordInput };
                var txtUser = dlg.GetTextField(0);
                var txtPass = dlg.GetTextField(1);

                txtUser.Placeholder = config.LoginPlaceholder;
                txtUser.Text = config.LoginValue ?? String.Empty;
                txtPass.Placeholder = config.PasswordPlaceholder;
                txtPass.SecureTextEntry = true;

                dlg.Clicked += (s, e) => {
                    var ok = (dlg.CancelButtonIndex != e.ButtonIndex);
                    config.OnResult(new LoginResult(txtUser.Text, txtPass.Text, ok));
                };
                dlg.Show();
            });
        }


        public override void Prompt(PromptConfig config) {
            this.Dispatch(() =>  {
                var result = new PromptResult();
                var dlg = new UIAlertView(config.Title ?? String.Empty, config.Message, null, config.CancelText, config.OkText) {
                    AlertViewStyle = config.InputType == InputType.Password
                        ? UIAlertViewStyle.SecureTextInput
                        : UIAlertViewStyle.PlainTextInput
                };
                var txt = dlg.GetTextField(0);
                txt.SecureTextEntry = config.InputType == InputType.Password;
                txt.Placeholder = config.Placeholder;
                txt.KeyboardType = Utils.GetKeyboardType(config.InputType);

                dlg.Clicked += (s, e) => {
                    result.Ok = (dlg.CancelButtonIndex != e.ButtonIndex);
                    result.Text = txt.Text;
                    config.OnResult(result);
                };
                dlg.Show();
            });
        }
    }
}