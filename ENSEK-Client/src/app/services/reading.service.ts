import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpRequest, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ReadingService
{
    constructor(private http: HttpClient) 
    {
    }

    uploadReading(serverIP:string, file:File) : Observable<HttpEvent<any>>
    {
        let formData = new FormData();
        formData.append('File', file);

        let params = new HttpParams();

        const options = {
            params: params,
            reportProgress: true,
          };

        const req = new HttpRequest('POST', serverIP + "/api/Reading/meter-reading-upload", formData, options);
        return this.http.request(req);
    }
}