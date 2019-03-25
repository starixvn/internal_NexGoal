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

    describe('click next', () => {
        const options = [{ value: 2, name: '2h' }, { value: 1, name: '1h' }];

        test('state change correctly', () => {
            const wrapper = shallow(
                <Milestone options={options} />
            );
            wrapper.setState({
                currentIndex: 1,
                nextDisabled: false,
                prevDisabled: true
            });

            wrapper.find('.arrow-right').simulate('click');
            expect(wrapper.state('currentIndex')).toEqual(0);
            expect(wrapper.state('nextDisabled')).toEqual(true);
            expect(wrapper.state('prevDisabled')).toEqual(false);
        });

        test('call props.onChange()', () => {
            const onChange = jest.fn();
            const wrapper = shallow(
                <Milestone options={options} onChange={onChange} />
            );
            wrapper.setState({
                currentIndex: 1,
                nextDisabled: false,
                prevDisabled: true
            });

            wrapper.find('.arrow-right').simulate('click');
            expect(onChange).toHaveBeenCalledTimes(1);
            expect(onChange).toHaveBeenLastCalledWith(2);
        });
    });

    describe('click previous', () => {
        const options = [{ value: 2, name: '2h' }, { value: 1, name: '1h' }];

        test('state change correctly', () => {
            const wrapper = shallow(
                <Milestone options={options} />
            );

            wrapper.find('.arrow-left').simulate('click');
            expect(wrapper.state('currentIndex')).toEqual(1);
            expect(wrapper.state('nextDisabled')).toEqual(false);
            expect(wrapper.state('prevDisabled')).toEqual(true);
        });

        test('call props.onChange()', () => {
            const onChange = jest.fn();
            const wrapper = shallow(
                <Milestone options={options} onChange={onChange} />
            );

            wrapper.find('.arrow-left').simulate('click');
            expect(onChange).toHaveBeenCalledTimes(1);
            expect(onChange).toHaveBeenLastCalledWith(1);
        });
    });
});