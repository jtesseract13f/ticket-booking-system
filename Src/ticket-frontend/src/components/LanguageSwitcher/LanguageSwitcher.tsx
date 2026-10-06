// src/components/LanguageSwitcher/LanguageSwitcher.tsx
import { useLang } from '../../i18n/LangContext';
import type { Lang } from '../../i18n/translations';

const LANGS: { code: Lang; label: string }[] = [
    { code: 'ru', label: 'RU' },
    { code: 'en', label: 'EN' },
    { code: 'kg', label: 'KG' },
];

interface Props {
    /** 'light' — для светлого хэдера, 'dark' — для тёмного (админка) */
    variant?: 'light' | 'dark';
}

export const LanguageSwitcher = ({ variant = 'light' }: Props) => {
    const { lang, setLang } = useLang();

    return (
        <div className={`lang-switcher lang-switcher--${variant}`} role="group" aria-label="Language">
            {LANGS.map((l) => (
                <button
                    key={l.code}
                    type="button"
                    className={`lang-switcher__btn ${lang === l.code ? 'is-active' : ''}`}
                    onClick={() => setLang(l.code)}
                    aria-pressed={lang === l.code}
                >
                    {l.label}
                </button>
            ))}
        </div>
    );
};