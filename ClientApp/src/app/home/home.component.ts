import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  public services: ServicesStatus;
  public telegramServicesWorkingText: string;
  public autoRunnerText: string;
  public webServiceConnectionText: string;
  public httpClient: HttpClient;
  public baseUrl: string;
  public numberToCheck: string;
  public checkedPhone: ILead[];
  public resultTableCreated: boolean;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {

    this.httpClient = http;
    this.baseUrl = baseUrl;
    this.resultTableCreated = false;
    this.checkedPhone = [
      { id: 0, phone: "", isTelegram: false, telegramUser: "", isViber: false, isWhatsApp: false }
    ];
    http.get<ServicesStatus>(baseUrl + 'Connection')
      .subscribe((result: ServicesStatus) => {
        var jsonResult = JSON.parse(JSON.stringify(result));
        this.autoRunnerText = jsonResult.autoRunner ? "Running" : "Offline";
        this.webServiceConnectionText = jsonResult.webServiceConnection ? "Web Service Online" : "Offline";
        this.telegramServicesWorkingText = jsonResult.telegramServicesWorking ? "Online" : "Offline";
      }, error => console.error(error));
  }

  public onKey(event) {
    if (event.target.id === "phone") {
      if (event.keyCode === 13) {
        this.checkNumber();
      } else {
        this.numberToCheck = event.target.value;
        return;
      }
    }
  }

  public checkNumber() {
    if (this.numberToCheck != null && this.numberToCheck.length > 0) {
      this.httpClient.post(this.baseUrl + 'Check',
        "[   {\"id\": 559,\"phone\": \"" + this.numberToCheck + "\"}]",
        { headers: { 'Content-Type': 'application/json' } }
      ).subscribe(function (response) {

        var searchResult: any[];
        var result = JSON.parse(JSON.stringify(response));
        if (result != null) {
          var resultCode = response['resultCode'];
          var isException = response['isException'];
          var messageText = response['messageText'];
          if (resultCode !== null && (resultCode === 401 || resultCode === 402 || resultCode === 403)) {
            alert("Служба TelegramService отключена, пожалуйста проверьте настройки и авторизируйтесь");
            window.location.href = document.baseURI + '/settingspage';
          }
          if (result.length > 0) {
            this.checkedPhone = [
              {
                id: result[0].id,
                phone: result[0].phone,
                isTelegram: result[0].isTelegram,
                isViber: result[0].isViber,
                isWhatsApp: result[0].isWhatsApp
              }
            ];
            var tr = document.createElement("tr");
            tr.id = "0";
            //var id = document.createElement("td");
            var phone = document.createElement("td");
            var isTelegram = document.createElement("td");
            var isViber = document.createElement("td");
            var isWhatsApp = document.createElement("td");

            // id.innerHTML = result[0].id;
            phone.innerHTML = result[0].phone;
            isTelegram.innerHTML = result[0].isTelegram ?
              'Есть <a href="tg://resolve?domain=' + result[0].telegramUser + ' "> <img src="assets/img/telegram.png" width="30px" height="30px"></a>' : 'Отсутствует';
            isViber.innerHTML = result[0].isViber ? 'Есть' : 'Отсутствует';
            isWhatsApp.innerHTML = result[0].isWhatsApp ? 'Есть' : 'Отсутствует';

            // tr.appendChild(id);
            tr.appendChild(phone);
            tr.appendChild(isTelegram);
            tr.appendChild(isViber);
            tr.appendChild(isWhatsApp);

            if (document.getElementById('table') === null) {
              var table = document.createElement('table');
              table.className = 'table table-striped';
              table.id = 'table';
              table.style.color = 'white;';

              var header = document.createElement("tr");
              var PhoneNumber = document.createElement("th");
              PhoneNumber.innerHTML = 'PhoneNumber';

              var Telegram = document.createElement("th");

              Telegram.innerHTML = 'Telegram ';

              var Viber = document.createElement("th");
              Viber.innerHTML = 'Viber';

              var WhatsApp = document.createElement("th");
              WhatsApp.innerHTML = 'WhatsApp';

              header.appendChild(PhoneNumber);
              header.appendChild(Telegram);
              header.appendChild(Viber);
              header.appendChild(WhatsApp);
              table.appendChild(header);
              this.resultTableCreated = true;
              document.getElementById('tableplace').appendChild(table);
            }
            document.getElementById('table').appendChild(tr);
          }
        }


        // parse result
      });
    }
  }

}

interface ServicesStatus {
  TelegramServicesWorking: boolean;
  AutoRunner: boolean;
  WebServiceConnection: boolean;
}

interface IAuthResponce {
  ResultCode: number,
  IsException: boolean,
  MessageText: string;
}

interface ILead {
  id: number;
  phone: string;
  isTelegram: boolean;
  telegramUser: string;
  isViber: boolean;
  isWhatsApp: boolean;
}
