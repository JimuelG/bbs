import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'certificateType',
  standalone: true
})
export class CertificateTypePipe implements PipeTransform {
  private numberMap: { [key: number]: string } = {
    0: 'Barangay Clearance',
    1: 'Certificate of Residency',
    2: 'Barangay Indigency',
    3: 'First Time Job Seeker',
    4: 'Good Moral',
    5: 'Small Business'
  };

  private stringMap: { [key: string]: string } = {
    Clearance: 'Barangay Clearance',
    Residency: 'Certificate of Residency',
    Indigency: 'Barangay Indigency',
    FirstTimeJobSeeker: 'First Time Job Seeker',
    GoodMoral: 'Good Moral',
    SmallBusiness: 'Small Business',

    BarangayClearance: 'Barangay Clearance',
    CertificateOfResidency: 'Certificate of Residency',
    BarangayIndigency: 'Barangay Indigency'
  };

  transform(value: number | string | null | undefined): string {
    if (value === null || value === undefined || value === '') {
      return 'Unknown';
    }

    if (typeof value === 'number') {
      return this.numberMap[value] || 'Unknown';
    }

    if (!isNaN(Number(value))) {
      return this.numberMap[Number(value)] || 'Unknown';
    }

    return this.stringMap[value] || this.formatEnumString(value);
  }

  private formatEnumString(value: string): string {
    return value
      .replace(/([a-z])([A-Z])/g, '$1 $2')
      .replace(/_/g, ' ')
      .trim();
  }
}