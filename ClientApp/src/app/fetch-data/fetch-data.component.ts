import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public leads: ILead[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private sanitizer: DomSanitizer) {
    http.get<ILead[]>(baseUrl + 'leadshistory').subscribe(result => {
      this.leads = result;

      if (document.getElementById('table') === null) {

        var table = document.createElement('table');
        table.className = 'table table-striped';
        table.id = 'table';
        table.style.color = 'white;';

        var header = document.createElement("tr");

        var id = document.createElement("th");
        id.innerHTML = 'ID';

        var PhoneNumber = document.createElement("th");
        PhoneNumber.innerHTML = 'PhoneNumber';

        var Telegram = document.createElement("th");
        Telegram.innerHTML = 'Telegram ';

        var Viber = document.createElement("th");
        Viber.innerHTML = 'Viber';

        var WhatsApp = document.createElement("th");
        WhatsApp.innerHTML = 'WhatsApp';

        header.appendChild(id);
        header.appendChild(PhoneNumber);
        header.appendChild(Telegram);
        header.appendChild(Viber);
        header.appendChild(WhatsApp);

        table.appendChild(header);

        document.getElementById('token').appendChild(table);
      }

      if (this.leads.length > 0) {
        for (var i = 0; i < this.leads.length; i++) {
          var lead = this.leads[i];


          var tr = document.createElement("tr");
          var id = document.createElement("td");
          var phone = document.createElement("td");
          var isTelegram = document.createElement("td");
          var isViber = document.createElement("td");
          var isWhatsApp = document.createElement("td");

          id.innerHTML = lead.id.toString();
          phone.innerHTML = lead.phone;
          isTelegram.innerHTML = lead.isTelegram ? 'Есть <a href="tg://resolve?domain=' + lead.telegramUser + '"><img src="assets/img/telegram.png" width="30px" height="30px"></a>' : 'Отсутствует';
          isTelegram.style.color = lead.isTelegram ? 'lightgreen' : 'pink';

          isViber.innerHTML = lead.isViber ? 'Есть' : 'Отсутствует';
          isViber.style.color = lead.isViber ? 'lightgreen' : 'pink';

          isWhatsApp.innerHTML = lead.isWhatsApp ? 'Есть' : 'Отсутствует';
          isWhatsApp.style.color = lead.isWhatsApp ? 'lightgreen' : 'pink';

          tr.appendChild(id);
          tr.appendChild(phone);
          tr.appendChild(isTelegram);
          tr.appendChild(isViber);
          tr.appendChild(isWhatsApp);

          table.appendChild(tr);
        }
      }
    }, error => console.error(error));
  }

  sanitize(url: string) {
    return this.sanitizer.bypassSecurityTrustUrl(url).toString();
  }
}

interface ILead {
  id: number;
  phone: string;
  isTelegram: boolean;
  telegramUser: string;
  isViber: boolean;
  isWhatsApp: boolean;
}
