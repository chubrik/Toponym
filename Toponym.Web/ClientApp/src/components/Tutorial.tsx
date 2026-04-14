import { getLanguage } from '../i18n/lang';
import { useStore } from '../state/store';
import { Language } from '../types';

interface Example {
  query: string;
  html: string;
}

function tutorial(lang: Language): { heading: string; examples: Example[]; wiki: { url: string; text: string; prefix: string; query: string } } {
  if (lang === Language.Belarusian) {
    return {
      heading: 'Прыклады запытаў:',
      examples: [
        { query: 'ала', html: 'змяшчае <b>ала</b>' },
        { query: '"гара"', html: 'дакладна адпавядае <b>гара</b>' },
        { query: 'ляс-', html: 'пачынаецца на <b>ляс</b>' },
        { query: '-аў/-ін', html: 'скончваецца на <b>аў</b> або <b>ін</b>' },
        { query: 'гуд-кі', html: 'пачынаецца на <b>гуд</b><br>і скончваецца на <b>кі</b>' },
      ],
      wiki: {
        query: '(?<!кі)$',
        prefix: 'гл. ',
        url: 'https://ru.wikipedia.org/wiki/Регулярные_выражения',
        text: 'рэгулярныя выразы',
      },
    };
  }
  if (lang === Language.English) {
    return {
      heading: 'Query examples:',
      examples: [
        { query: 'olo', html: 'contains <b>olo</b>' },
        { query: '"gora"', html: 'exactly matches <b>gora</b>' },
        { query: 'les-', html: 'starting with <b>les</b>' },
        { query: '-ov/-in', html: 'ending with <b>ov</b> or <b>in</b>' },
        { query: 'gud-ki', html: 'starting with <b>gud</b><br>and ending with <b>ki</b>' },
      ],
      wiki: {
        query: '(?<!ki)$',
        prefix: 'see ',
        url: 'https://en.wikipedia.org/wiki/Regular_expression',
        text: 'regular expression',
      },
    };
  }
  return {
    heading: 'Примеры запросов:',
    examples: [
      { query: 'оло', html: 'содержит <b>оло</b>' },
      { query: '"гора"', html: 'точно соответствует <b>гора</b>' },
      { query: 'лес-', html: 'начинается на <b>лес</b>' },
      { query: '-ов/-ин', html: 'оканчивается на <b>ов</b> или <b>ин</b>' },
      { query: 'гуд-ки', html: 'начинается на <b>гуд</b><br>и оканчивается на <b>ки</b>' },
    ],
    wiki: {
      query: '(?<!ки)$',
      prefix: 'см. ',
      url: 'https://ru.wikipedia.org/wiki/Регулярные_выражения',
      text: 'регулярные выражения',
    },
  };
}

export function Tutorial() {
  const setGroupValue = useStore((s) => s.setGroupValue);
  const currentGroupIndex = useStore((s) => s.currentGroupIndex);
  const data = tutorial(getLanguage());

  const setExample = (q: string) => setGroupValue(currentGroupIndex, q);

  return (
    <div className="tutorial">
      <p>{data.heading}</p>
      {data.examples.map((ex) => (
        <p key={ex.query}>
          <a
            className="example"
            href=""
            tabIndex={-1}
            onClick={(e) => {
              e.preventDefault();
              setExample(ex.query);
            }}
          >
            {ex.query}
          </a>
          <br />
          <span dangerouslySetInnerHTML={{ __html: ex.html }} />
        </p>
      ))}
      <p>
        <a
          className="example"
          href=""
          tabIndex={-1}
          onClick={(e) => {
            e.preventDefault();
            setExample(data.wiki.query);
          }}
        >
          {data.wiki.query}
        </a>
        <br />
        {data.wiki.prefix}
        <a href={data.wiki.url} target="_blank" tabIndex={-1} rel="noreferrer">
          {data.wiki.text}
        </a>
      </p>
    </div>
  );
}
