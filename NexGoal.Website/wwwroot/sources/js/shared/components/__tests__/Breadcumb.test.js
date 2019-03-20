import React from 'react';
import renderer from 'react-test-renderer';
import { shallow } from 'enzyme';
import Breadcrumb from '../Breadcrumb';

describe('<Breadcrumb />', () => {
    test('render', () => {
        const wrapper = renderer.create(
            <Breadcrumb />,
        );

        let tree = wrapper.toJSON();
        expect(tree).toMatchSnapshot();
    });

    test('renders children when passed in', () => {
        const children = <div>test</div>;
        const wrapper = shallow(
            <Breadcrumb>
                {children}
            </Breadcrumb>
        );

        expect(wrapper.contains(children)).toBeTruthy();
    });
});