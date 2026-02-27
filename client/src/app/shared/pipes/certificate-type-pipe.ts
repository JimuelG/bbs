import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'certificateType',
})
export class CertificateTypePipe implements PipeTransform {
  private typeMap: { [key: number]: string} = {
      1: 'Clearance',
      2: 'Residency',
      3: 'Indigency',
      4: 'Indigency V2',
      5: 'First Time Job Seeker',
      6: 'First Time Job Seeker V2',
      7: 'Death Certificate',
      8: 'Farmer Loan',
      9: 'Good Moral',
      10: 'Letter of Acceptance',
      11: 'National ID',
      12: 'Burial Assistance',
      13: 'No Birth Certificate',
      14: 'Overseas Certificate',
      15: 'Senior Certificate',
      16: 'Small Business',
      17: 'Tree Cutting'
  }

  transform(value: number): string {
    return this.typeMap[value] || 'Inknown';
  }

}
