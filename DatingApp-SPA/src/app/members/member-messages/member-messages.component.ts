import { AlertifyService } from './../../_services/alertify.service';
import { AuthService } from './../../_services/auth.service';
import { UserService } from './../../_services/user.service';
import { Message } from './../../_models/message';
import { User } from './../../_models/user';
import { Component, OnInit, Input } from '@angular/core';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipient: User;
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService,
              private authService: AuthService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    const userId = +this.authService.decodedToken.nameid;
    const recipientId = this.recipient.id;

    this.userService.getMessageThread(userId, recipientId)
      .pipe(
        tap(messages => {
          //debugger;
          for(let i = 0; i < messages.length; i++) {
            if (messages[i].isRead === false && messages[i].recipientId === userId) {
              this.userService.markMessageAsRead(messages[i].id, userId);
            }
          }
        }
        )
      )
      .subscribe(resp => {
        this.messages = resp;
      }, err => {
        this.alertify.error(err);
      });
  }

  sendMessage() {
    if (JSON.stringify(this.newMessage) === '{}') {
      return;
    }

    this.newMessage.recipientId = this.recipient.id;

    // this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage).subscribe((resp: Message) => {
    //   this.messages.unshift(resp);
    //   //this.newMessage.content = '';
    // }, err => {
    //   this.alertify.error(err);  
    // });

    this.userService.sendMessageRevised(this.authService.decodedToken.nameid, this.newMessage).subscribe(resp => {
      resp.subscribe((re: Message) => {
        //console.log(re);
        this.messages.unshift(re);
        this.newMessage.content = '';
      }, err => {
        this.alertify.error(err);
      });
    }, err => {
      this.alertify.error(err);
    });
  }
}
