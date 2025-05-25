import { Photo } from "./photo"

export interface Member {
  id: number
  userName: string
  age: number
  knownAs: string
  photoUrl: string
  created: Date
  lastActive: Date
  gender: string
  introduction: string
  interests: string
  lookingFor: string
  city: string
  country: string
  photos: Photo[]
}


