import React from 'react';
import HRMSummary from '../HRMSummary';
import renderer from 'react-test-renderer';
import { shallow, mount } from 'enzyme';

describe('renders correctly', () => {
    it('render UI', () => {
        const component = renderer.create(<HRMSummary />);
        expect(component.toJSON()).toMatchSnapshot();
    });

    it('component should did mount', () => {
        const spy = jest.spyOn(HRMSummary.prototype, 'componentDidMount');
        const wrapper = mount(<HRMSummary />);
        expect(spy).toHaveBeenCalled();
        spy.mockReset();
        spy.mockRestore();
    });
});