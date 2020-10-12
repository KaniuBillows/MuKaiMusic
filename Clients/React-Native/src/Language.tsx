
//TODO: implement multi languages support.
import React, { createContext, useState } from "react";
import { ChinesePack, EnglishPack, LanguagePack } from "./util/LanguagePack";

export const LanguageContext = createContext<LanguagePack>(ChinesePack);

let _setState: React.Dispatch<React.SetStateAction<LanguagePack>>
export function MultiLan(props) {
    const [language, setState] = useState<LanguagePack>(ChinesePack);
    _setState = setState;
    return (<LanguageContext.Provider value={language}>{props.children}</LanguageContext.Provider>)
}
export function setLanguage(lan: 'english' | 'chinese') {
    _setState(lan === 'chinese' ? ChinesePack : EnglishPack);
}