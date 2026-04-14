import { useEffect, useRef } from 'react';

export function useDebouncedCallback<T extends (...args: never[]) => void>(
  fn: T,
  delayMs: number,
): { invoke: (...args: Parameters<T>) => void; cancel: () => void } {
  const ref = useRef(fn);
  const timer = useRef<number | null>(null);

  useEffect(() => {
    ref.current = fn;
  }, [fn]);

  useEffect(() => () => {
    if (timer.current !== null) window.clearTimeout(timer.current);
  }, []);

  const invoke = (...args: Parameters<T>) => {
    if (timer.current !== null) window.clearTimeout(timer.current);
    timer.current = window.setTimeout(() => {
      timer.current = null;
      ref.current(...args);
    }, delayMs);
  };

  const cancel = () => {
    if (timer.current !== null) {
      window.clearTimeout(timer.current);
      timer.current = null;
    }
  };

  return { invoke, cancel };
}
