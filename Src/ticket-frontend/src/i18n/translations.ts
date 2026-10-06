// src/i18n/translations.ts
export type Lang = 'ru' | 'en' | 'kg';

export const translations = {
    ru: {
        admin: 'Админ-панель',
        home: 'На главную',
        users: 'Пользователи',
        nodes: 'Узлы системы',
        link: 'Ссылка',
        status: 'Статус',
        interface: 'Интерфейс',
        on: 'Вкл',
        off: 'Выкл',
        loading: 'Загрузка...',
        noAccess: 'Доступ запрещён',
        login: 'Войти',
        logout: 'Выйти',
        search: 'Поиск',
        interfaceNames: {
            CinemaApi: 'CinemaApi',
            ConcertApi: 'ConcertApi',
            TicketApi: 'TicketApi'
        },
    },
    en: {
        admin: 'Admin panel',
        home: 'Home',
        users: 'Users',
        nodes: 'System nodes',
        link: 'Link',
        status: 'Status',
        interface: 'Interface',
        on: 'On',
        off: 'Off',
        loading: 'Loading...',
        noAccess: 'Access denied',
        login: 'Sign in',
        logout: 'Sign out',
        search: 'Search',
        interfaceNames: {
            CinemaApi: 'CinemaApi',
            ConcertApi: 'ConcertApi',
            TicketApi: 'TicketApi',
        },
    },
    kg: {
        admin: 'Админ-панель',
        home: 'Башкы бетке',
        users: 'Колдонуучулар',
        nodes: 'Система түйүндөрү',
        link: 'Шилтеме',
        status: 'Абалы',
        interface: 'Интерфейс',
        on: 'Күйгүзүлгөн',
        off: 'Өчүрүлгөн',
        loading: 'Жүктөлүүдө...',
        noAccess: 'Кирүүгө тыюу салынган',
        login: 'Кирүү',
        logout: 'Чыгуу',
        search: 'Издөө',
        interfaceNames: {
            CinemaApi: 'CinemaApi',
            ConcertApi: 'ConcertApi',
            TicketApi: 'TicketApi',
        },
    },
} as const;

export type Translation = typeof translations['ru'];