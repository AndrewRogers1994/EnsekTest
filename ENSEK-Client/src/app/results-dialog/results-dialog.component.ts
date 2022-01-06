import {Component, Inject} from '@angular/core';
import {MatDialog, MAT_DIALOG_DATA} from '@angular/material/dialog';

@Component({
  selector: 'results-dialog',
  templateUrl: './results-dialog.component.html',
  styleUrls: ['./results-dialog.component.css']
})
export class ResultsDialog 
{
  counts = [{"successCount":1, "errorCount":2}];
  countsColumns: string[] = ['successCount', 'errorCount'];

  errors = undefined;// [{"Row":1, "Error":"This is a test message"}];
  errorColumns: string[] = ['rowID', 'message'];

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) 
  {
    console.log(data);
    this.counts = [{"successCount":data.successCount, "errorCount":data.errorCount}]
    this.errors = data.errorMessages;
  }
}