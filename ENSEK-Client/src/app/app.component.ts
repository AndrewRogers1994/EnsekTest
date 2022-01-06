import { HttpEventType, HttpResponse } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ResultsDialog } from './results-dialog/results-dialog.component';
import { ErrorDialog } from './error-dialog/error-dialog.component';
import { ReadingService } from './services/reading.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit
{
  @ViewChild('imgFileInput')
  fileInput:ElementRef;


  constructor(private matDialog: MatDialog, readingService: ReadingService)
  {
    this.readingService = readingService;
  }

  title = 'ENSEK-Client';
  selectedFile = undefined;
  serverIPAddress:string = "http://localhost:5001";
  processingRequest:boolean = false;
  fileUploadProgress = 100;
  readingService:ReadingService;

  ngOnInit(): void 
  {
     var ip = localStorage.getItem('serverIPAddress');
     if(ip != undefined)
     {
       this.serverIPAddress = ip;
     }
  }

  fileSelected(event)
  {
    if(event.target.files.length > 0)
    {
      this.selectedFile = event.target.files[0];
    }
    else
    {
      this.selectedFile = undefined;
    }
  }

  uploadReadings()
  {
    //Reset any progress bar
    this.fileUploadProgress = 0;
    this.processingRequest = true;

    //Traim any trailing slashes from the url
    while(this.serverIPAddress.charAt(this.serverIPAddress.length-1) == "/")
    {
      console.log("trimming char");
      this.serverIPAddress = this.serverIPAddress.slice(0,-1);
    }

    //Update the serverIPAddress in local storage so it's retained on page refresh
    localStorage.setItem('serverIPAddress', this.serverIPAddress);

    //Start the file upload
    this.readingService.uploadReading(this.serverIPAddress, this.selectedFile).subscribe(event =>
      {
        if(event.type == HttpEventType.UploadProgress)
        {
          const percentDone = Math.round(100 * event.loaded / event.total);
          this.fileUploadProgress = percentDone;
        }
        else if(event instanceof HttpResponse)
        {
          //Remove the chosen file
          this.selectedFile = undefined;
          this.fileInput.nativeElement.value = "";

          const dialogConfig = new MatDialogConfig();
          dialogConfig.data = event.body;
          this.matDialog.open(ResultsDialog, dialogConfig);
          this.processingRequest = false;
        }
      }, 
      (error) =>
      {
        console.log(error);
        const dialogConfig = new MatDialogConfig();
        dialogConfig.data = {message:error.message}
        this.matDialog.open(ErrorDialog, dialogConfig);
        this.processingRequest = false;
      })
  }
}