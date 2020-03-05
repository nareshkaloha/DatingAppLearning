import { AuthService } from './../_services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from './../_services/alertify.service';
import { UserService } from './../_services/user.service';
import { Pagination, PaginatedResult } from './../_models/pagination';
import { Message } from './../_models/message';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';

  constructor(private userService: UserService,
              private alertify: AlertifyService,
              private route: ActivatedRoute,
              private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(d => {
      this.messages = d['messages'].result;
      this.pagination = d['messages'].pagination;
    });
  }

  loadMessages() {
    this.userService.getMessages(this.authService.decodedToken.nameid,
      this.pagination.currentPage,
      this.pagination.itemsPerPage,
      this.messageContainer).subscribe( (resp: PaginatedResult<Message[]>) => {
        this.messages = resp.result;
        this.pagination = resp.pagination;
      }, err => {
        this.alertify.error(err);
      });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

  deleteMessage(messageId: number) {
    this.alertify.confirm('Are you sure you want to delete this message ?', () => {
      this.userService.deleteMessage(messageId, this.authService.decodedToken.nameid).subscribe(() => {
        this.messages.splice(this.messages.findIndex(f => f.id === messageId), 1);
        this.alertify.success('message deleted ..');
      }, err => {
        this.alertify.error(err);
      });
    });
  }
}
