// src/i18n/LangContext.tsx
import { createContext, useContext, useState, type ReactNode } from 'react';
import { translations, type Lang, type Translation } from './translations';

const SUPPORTED: Lang[] = ['ru', 'en', 'kg'];

interface LangCtx {
    lang: Lang;
    t: Translation;
    setLang: (l: Lang) => void;
}

const Ctx = createContext<LangCtx | null>(null);

export const LangProvider = ({ children }: { children: ReactNode }) => {
    const [lang, setLangState] = useState<Lang>(() => {
        const saved = localStorage.getItem('lang') as Lang | null;
        return saved && SUPPORTED.includes(saved) ? saved : 'ru';
    });

    const setLang = (l: Lang) => {
        localStorage.setItem('lang', l);
        setLangState(l);
    };

    return (
        <Ctx.Provider value={{ lang, t: translations[lang], setLang }}>
            {children}
        </Ctx.Provider>
    );
};

export function useLang() {
    const ctx = useContext(Ctx);
    if (!ctx) throw new Error('useLang must be used inside LangProvider');
    return ctx;
}