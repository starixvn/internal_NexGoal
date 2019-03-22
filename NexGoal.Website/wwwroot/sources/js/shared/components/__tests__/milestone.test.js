import React from 'react';
import renderer from 'react-test-renderer';
import { shallow } from 'enzyme';
import Milestone from '../milestone';

describe('<Milestone />', () => {
    test('renders correctly', () => {
        const wrapper = renderer.create(
            <Milestone />,
        );

        expect(wrapper.toJSON()).toMatchSnapshot();
    });

    test('renders children when options is empty', () => {
        const wrapper = shallow(
            <Milestone options={[]} />
        );

        expect(wrapper.find('.arrow-left.disabled')).toHaveLength(1);
        expect(wrapper.find('.arrow-right.disabled')).toHaveLength(1);
    });

    test('simulates onchange events', () => {
        const options = [{ value: 2, name: '2h' }, { value: 1, name: '1h' }];
        const onChange = jest.fn();
        const wrapper = shallow(
            <Milestone options={options} onChange={onChange} />
        );

        wrapper.find('.arrow-left').simulate('click');
        expect(onChange).toHaveBeenCalledTimes(1);
        expect(onChange).toHaveBeenLastCalledWith(1);
    });
});