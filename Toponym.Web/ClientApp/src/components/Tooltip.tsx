import {
  FloatingPortal,
  flip,
  offset,
  shift,
  useFloating,
  useHover,
  useInteractions,
} from '@floating-ui/react';
import { cloneElement, isValidElement, useState } from 'react';
import type { Placement } from '@floating-ui/react';
import type { ReactElement, Ref } from 'react';

interface Props {
  label: string;
  placement?: Placement;
  children: ReactElement<{ ref?: Ref<Element>; onMouseEnter?: unknown; onMouseLeave?: unknown }>;
}

export function Tooltip({ label, placement = 'top', children }: Props) {
  const [open, setOpen] = useState(false);
  const { refs, floatingStyles, context } = useFloating({
    open,
    onOpenChange: setOpen,
    placement,
    middleware: [offset(6), flip(), shift({ padding: 5 })],
  });
  const hover = useHover(context, { delay: 0 });
  const { getReferenceProps, getFloatingProps } = useInteractions([hover]);

  if (!isValidElement(children)) return children;
  const childProps = (children.props ?? {}) as Record<string, unknown>;
  const refProps = getReferenceProps() as Record<string, unknown>;
  const merged: Record<string, unknown> = { ...childProps, ...refProps };
  for (const key of Object.keys(refProps)) {
    const tooltipFn = refProps[key];
    const childFn = childProps[key];
    if (typeof tooltipFn === 'function' && typeof childFn === 'function') {
      merged[key] = (...args: unknown[]) => {
        (tooltipFn as (...a: unknown[]) => unknown)(...args);
        (childFn as (...a: unknown[]) => unknown)(...args);
      };
    }
  }
  const child = cloneElement(children, {
    ...merged,
    ref: refs.setReference as Ref<Element>,
  });

  return (
    <>
      {child}
      {open && label && (
        <FloatingPortal>
          <div
            ref={refs.setFloating}
            style={{
              ...floatingStyles,
              background: 'rgba(0,0,0,0.8)',
              color: 'white',
              padding: '2px 6px',
              borderRadius: 3,
              fontSize: 12,
              pointerEvents: 'none',
              zIndex: 9999,
            }}
            {...getFloatingProps()}
          >
            {label}
          </div>
        </FloatingPortal>
      )}
    </>
  );
}
