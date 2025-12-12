import { User } from './interface';

export const admin: User = {
  id: 1,
  username: 'testuser',
  displayName: 'Test User',
  email: 'test@example.com',
  roles: ['Admin'],
  avatar: 'images/avatar.jpg',
};

export const guest: User = {
  username: 'unknown',
  displayName: 'Guest',
  email: 'unknown',
  roles: [],
  avatar: 'images/avatar-default.jpg',
};
