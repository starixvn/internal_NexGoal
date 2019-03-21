import React from 'react';
import { shallow, mount } from 'enzyme';
import WorkLoad from '../workload';
import renderer from 'react-test-renderer';
import mockAxios from 'axios';

jest.mock('axios');

const workloads = [
    { projectId: 1, name: 'HRM', totalHours: 10 },
    { projectId: 2, name: 'SMS HRM', totalHours: 14 },
    { projectId: 3, name: 'Nexgoal', totalHours: 21 },
    { projectId: 4, name: 'HRP', totalHours: 40 }
];
const periods = [{ periodId: 1 }, { periodId: 2 }, { periodId: 3 }];

describe('Personal workload component', () => {
    beforeEach(() => {
        mockAxios.get.mockImplementation(() =>
            Promise.resolve({
                data: {
                    workloads: workloads
                }
            })
        );
    });

    afterEach(() => {
        mockAxios.get.mockReset();
    });

    describe('Component render correctly', () => {
        it('renderUI correctly', () => {
            const wrapper = renderer.create(<WorkLoad periods={periods} />);
            expect(wrapper.toJSON()).toMatchSnapshot();
        });

        it('Axios should be called on did mount', () => {
            const wrapper = mount(<WorkLoad periods={periods} />);
            expect(mockAxios.get).toHaveBeenCalledTimes(1);
        });
    });

    it('State should be changed when click next, prev period', () => {
        let component = shallow(<WorkLoad periods={periods} />);
        component.setState({
            periodId: 1
        });
        component.instance().onPeriodChange(2);
        expect(component.state().periodId).toBe(2);
        expect(mockAxios.get).toHaveBeenCalledTimes(2);
    });
});