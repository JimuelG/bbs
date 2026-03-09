export type User = {
    id: string,
    firstName: string,
    middleName?: string,
    lastName: string,
    email: string,
    pictureUrl?: string,
    phoneNumber: string,
    isIdVerified: boolean,
    idUrl: string,
    role: string
}