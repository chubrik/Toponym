import { getLanguage, langText, rusCase } from '../i18n/lang';
import { useStore } from '../state/store';
import { Language } from '../types';

interface Props {
  fbAppId: string;
  defaultHost: string;
}

function canonicalUrl(defaultHost: string): string {
  return 'https://' + defaultHost + window.location.pathname + window.location.search;
}

function tweetText(firstQuery: string, matchCount: number): string {
  const lang = getLanguage();
  if (!firstQuery) {
    return langText(
      'Смотрите, что есть на карте Беларуси!',
      'Глядзіце, што ёсць на карце Беларусі!',
      'Look what is on the map of Belarus!',
    );
  }

  const foundRu =
    rusCase(matchCount, ['топоним', 'топонима', 'топонимов']) + ` "${firstQuery}"`;
  const foundBe =
    rusCase(matchCount, ['тапонім', 'тапоніма', 'тапонімаў']) + ` "${firstQuery}"`;
  const foundEn =
    matchCount + (matchCount === 1 ? ' toponym' : ' toponyms') + ` "${firstQuery}"`;

  const ru =
    rusCase(matchCount, ['Найден', 'Найдено', 'Найдено'], false) +
    ` ${foundRu} на карте Беларуси.`;
  const be =
    rusCase(matchCount, ['Знойдзены', 'Знойдзена', 'Знойдзена'], false) +
    ` ${foundBe} на карце Беларусі.`;
  const en = `Found ${foundEn} on the map Belarus.`;

  switch (lang) {
    case Language.Russian: return ru;
    case Language.Belarusian: return be;
    case Language.English: return en;
  }
}

export function ShareButtons({ fbAppId, defaultHost }: Props) {
  const firstValue = useStore((s) => s.groups[0]?.value ?? '');
  const firstMatchCount = useStore((s) => s.groups[0]?.matchCount ?? 0);

  const fbUrl =
    `https://www.facebook.com/dialog/share?display=popup&app_id=${fbAppId}` +
    `&href=${encodeURIComponent(canonicalUrl(defaultHost))}`;
  const vkUrl = `https://vk.ru/share.php?url=${encodeURIComponent(canonicalUrl(defaultHost))}`;

  const tweet = tweetText(firstValue, firstMatchCount);
  const tags = langText(
    '#топоним #топонимика #беларусь #белоруссия',
    '#топоним #топонимика #беларусь #белоруссия',
    '#toponym #toponymy #belarus #belorussia',
  );
  const twUrl =
    'https://twitter.com/intent/tweet?text=' +
    encodeURIComponent(`${tweet}\n\n${canonicalUrl(defaultHost)}\n${tags}`);

  const onClick = (e: React.MouseEvent<HTMLAnchorElement>, suffix: string) => {
    e.preventDefault();
    window.open(e.currentTarget.href, 'share_' + suffix, 'width=600,height=400');
  };

  return (
    <div id="socials">
      <a className="fb" href={fbUrl} target="_blank" rel="noreferrer" onClick={(e) => onClick(e, 'fb')}>
        <img src="/assets/img/icon-fb.png" alt="" />{' '}
        {langText('опубликовать', 'апублікаваць', 'publish')}
      </a>
      <a className="vk" href={vkUrl} target="_blank" rel="noreferrer" onClick={(e) => onClick(e, 'vk')}>
        <img src="/assets/img/icon-vk.png" alt="" />{' '}
        {langText('поделиться', 'падзяліцца', 'share')}
      </a>
      <a className="tw" href={twUrl} target="_blank" rel="noreferrer" onClick={(e) => onClick(e, 'tw')}>
        <img src="/assets/img/icon-tw.png" alt="" />{' '}
        {langText('твитнуть', 'твітнуць', 'tweet')}
      </a>
    </div>
  );
}
