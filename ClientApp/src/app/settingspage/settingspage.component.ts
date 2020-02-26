import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-settingspage-component',
  templateUrl: './settingspage.component.html'
})
export class SettingsPageComponent {

  public httpClient: HttpClient;
  public baseUrl: string;
  public serviceRunning: boolean;
  public apiId: string;
  public apiHash: string;
  public numberToAuthenticate: string;
  public codeToAuthenticate: string;
  public SourceUrl: string;
  public interval: number;
  public telegramSettingsChanging: boolean;
  public apiSettingsChanging: boolean;


  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.telegramSettingsChanging = false;
    this.apiSettingsChanging = false;
    this.telegramSettingsDisable(true);
    http.get(baseUrl + 'api/GetSettings')
      .subscribe((result: IServiceSettings) => {
        var jsonResult = JSON.parse(JSON.stringify(result));
        var parsedJsonResult = JSON.parse(jsonResult);
        var apihash = parsedJsonResult["ApiHash"];
        var apiId = parsedJsonResult["ApiId"];
        var numberToAuthenticate = parsedJsonResult["NumberToAuthenticate"];
        var codeToAuthenticate = parsedJsonResult["CodeToAuthenticate"];
        var interval = parsedJsonResult["Interval"];
        var sourceUrl = parsedJsonResult["SourceUrl"];
        this.serviceRunning = parsedJsonResult["Running"];

        if (apiId.length > 0 && apiId !== 'null') {
          document.getElementById('apiid').setAttribute('value', apiId);
          this.apiId = apiId;
        }

        if (apihash != null) {
          if (apihash.length > 0 && apihash !== 'null') {
            document.getElementById('apihash').setAttribute('value', apihash);
            this.apiHash = apihash;
          }
        }

        if (numberToAuthenticate.length > 0 && numberToAuthenticate !== 'null') {
          document.getElementById('numberToAuthenticate').setAttribute('value', numberToAuthenticate);
          this.numberToAuthenticate = numberToAuthenticate;

        }
        if (codeToAuthenticate.length > 0 && codeToAuthenticate !== 'null') {
          document.getElementById('codeToAuthenticate').setAttribute('value', codeToAuthenticate);
          this.codeToAuthenticate = codeToAuthenticate;
        }
        if (interval > 0) {
          document.getElementById('interval').setAttribute('value', interval);
          this.interval = interval;
        }
        if (sourceUrl.length > 0 && sourceUrl !== 'null') {
          document.getElementById('SourceUrl').setAttribute('value', sourceUrl);
          this.SourceUrl = sourceUrl;
        }
      }, error => console.error(error));
  }

  public onKey(event) {
    if (event.target.id === "apiid") {
      this.apiId = event.target.value;
      return;
    }
    if (event.target.id === "apihash") {
      this.apiHash = event.target.value;
      return;
    }
    if (event.target.id === "numberToAuthenticate") {
      this.numberToAuthenticate = event.target.value;
      return;
    }
    if (event.target.id === "codeToAuthenticate") {
      this.codeToAuthenticate = event.target.value;
      return;
    }
    if (event.target.id === "SourceUrl") {
      this.SourceUrl = event.target.value;
      return;
    }
    if (event.target.id === "interval") {
      this.interval = event.target.value;
    }
  }

  public sendPassword() {
    if (this.telegramSettingsChanging) {
      if (this.apiId.length > 0 && this.apiHash.length > 0 && this.numberToAuthenticate.length > 0) {
        var settings: string;
        settings = "{ " +
          "\"ApiId\": \"" +
          this.apiId +
          "\"," +
          "\"ApiHash\": \"" +
          this.apiHash +
          "\"," +
          "\"NumberToAuthenticate\": \"" +
          this.numberToAuthenticate +
          "\"," +
          "\"CodeToAuthenticate\": \"" +
          this.codeToAuthenticate +
          "\"" +
          "} ";
        this.httpClient.post(this.baseUrl + 'Auth',
          settings,
          { headers: { 'Content-Type': 'application/json' } }
        ).subscribe((responce: IAuthResponce) => {

          var responceJson = JSON.parse(JSON.stringify(responce));
          var codeResult = responceJson["resultCode"];
          var message = responceJson["messageText"];
          if (codeResult === 403) {
            alert('Пароль для двуфакторной авторизации отправлен в Telegram.\nПожалуйста, укажите пароль из сообщения');
          }
          if (codeResult === 200) {
            alert('Авторизация успешно выполнена, настройки сохранены, дополнительный действий не требуется');
          }
          if (codeResult === 401) {
            alert(
              'Неправильно указаны настройки для Telegram служб. Пожалуйста, укажите правильные ApiId, ApiHash, телефон и код авторизации');
          }
        });
      } else {
        alert(
          'Неправильно указаны настройки для Telegram служб. Пожалуйста, укажите правильные ApiId, ApiHash, телефон и код авторизации');
      }
    } else {
      this.telegramSettingsChanging = true;
      this.telegramSettingsDisable(false);
    }
  }

  public telegramSettingsDisable(disabled: boolean) {
  }

  public incrementCounter() { }
  public checkRunning() {
    this.sendPassword();
    this.httpClient.get<ServicesStatus>(this.baseUrl + 'Connection')
      .subscribe((result: ServicesStatus) => {
        var jsonResult = JSON.parse(JSON.stringify(result));
        var message = "";
        if (jsonResult.telegramServicesWorking) {
          message = "Подключение к сервису Telegram установлено.\nАвторизация выполнена";
        } else {
          message = "Подключение к сервису Telegram не установлено.\nАвторизация не выполнена";
        }
        alert(message);
      }, error => console.error(error));
  }

  public saveSettings() {
    if (this.apiSettingsChanging) {
      this.apiSettingsChanging = false;
      if (this.SourceUrl.length > 0 && this.interval > 0) {
        var settings: string;
        settings = "{ " +
          "\"Interval\": " +
          this.interval +
          "," +
          "\"Running\":" +
          this.serviceRunning +
          "," +
          "\"SourceUrl\": \"" +
          this.SourceUrl +
          "\"" +
          "} ";
        this.httpClient.post(this.baseUrl + 'Settings', settings, { headers: { 'Content-Type': 'application/json' } })
          .subscribe((responce: IAuthResponce) => {
            var responceJson = JSON.parse(JSON.stringify(responce));
            var codeResult = responceJson["resultCode"];
            var message = responceJson["messageText"];
            if (codeResult === 201) {
              alert('Настройки успешно сохранены');
            } else {
              alert('Неправильно указаны настройки. Пожалуйста, укажите правильные данные');
            }
          });
      } else {
        alert('Неправильно указаны настройки. Пожалуйста, укажите правильные данные');
      }
    } else {
      this.apiSettingsChanging = true;
    }
  }

  public webserviceSetup() {
    if (this.serviceRunning) {
      this.serviceRunning = false;
    } else {
      this.serviceRunning = true;
    }
    this.saveSettings();
  }
}

interface IServiceSettings {
  apiHash: string,
  apiId: string,
  numberToAuthenticate: string,
  codeToAuthenticate: string,
  interval: number,
  sourceUrl: string,
  running: boolean;
}

interface IAuthResponce {
  ResultCode: number,
  IsException: boolean,
  MessageText: string;
}

interface ServicesStatus {
  TelegramServicesWorking: boolean,
  AutoRunner: boolean,
  WebServiceConnection: boolean;
}


