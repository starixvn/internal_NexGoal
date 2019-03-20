import React from 'react';
import renderer from 'react-test-renderer';
import { shallow } from 'enzyme';
import StickyBlock from '../StickyBlock';

describe('Sticky Block', () => {
    test('Render', () => {
        const component = renderer.create(
            <StickyBlock threshold={200}>Test</StickyBlock>,
        );

        let tree = component.toJSON();
        expect(tree).toMatchSnapshot();
    });

    test('Render Children', () => {
        let children = <div>Test</div>;
        const component = shallow(
            <StickyBlock threshold={200}>
                {children}
            </StickyBlock>
        );

        expect(component.contains(children)).toBeTruthy();
    });

    test('Default is hidden', () => {
        const component = shallow(
            <StickyBlock threshold={200} />
        );

        expect(component.state().showStickyHeader).toBeFalsy();
    });

    test('window added scroll event', () => {
        const spy = jest.spyOn(StickyBlock.prototype, 'componentDidMount');
        const addEventListenerSpy = jest.spyOn(global, 'addEventListener');

        const component = shallow(
            <StickyBlock threshold={200}>
            </StickyBlock>
        );

        expect(spy).toHaveBeenCalled();
        expect(addEventListenerSpy).toHaveBeenCalled();
    });

    test('trigger handleToggleSticky', () => {
        const component = shallow(
            <StickyBlock threshold={200} />
        );

        global.scrollY = 300;
        component.instance().handleToggleSticky();
        expect(component.state().showStickyHeader).toBeTruthy();
        expect(component.hasClass('hidden')).toBeFalsy();
    });

    test('unmount', () => {
        const spy = jest.spyOn(StickyBlock.prototype, 'componentWillUnmount');
        const removeEventListenerSpy = jest.spyOn(global, 'removeEventListener');

        const component = shallow(
            <StickyBlock threshold={200} />
        );

        component.unmount();

        expect(spy).toHaveBeenCalled();
        expect(removeEventListenerSpy).toHaveBeenCalled();
    });
});