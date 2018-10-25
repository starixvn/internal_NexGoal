import React from "react";
import { shallow, mount } from "enzyme";
import NominationResult from '../NominationResult';
import renderer from 'react-test-renderer';
import axios from "axios";

jest.mock('axios');

const nominations = [{
    eoqyId: 1,
    nominationPeriodId: 111,
    period: {
        nominationId: 111,
        title: "Employee of the year 2018"
    },
    nominationDetails: [{ nominatorId: 363, comment: '' }, { nominatorId: 125, comment: '' }],
    statusId: 1
}, {
    eoqyId: 3,
    nominationPeriodId: 222,
    period: {
        nominationId: 222,
        title: "Employee of the quarter 3/2018"
    },
    nominationDetails: [{ nominatorId: 125, comment: '' }, { nominatorId: 363, comment: '' }],
    statusId: 2
}];

describe("Test render correctly ", () => {
    beforeEach(() => {
        axios.get.mockImplementation(() =>
            Promise.resolve({
                data: nominations
            })
        );
    });

    afterEach(() => {
        axios.get.mockReset();
    });

    describe('Component render correctly', () => {
        it("should fetch data when mount", async () => {
            const wrapper = mount(<NominationResult />);
            expect(axios.get).toHaveBeenCalledTimes(1);
        });

        it('renderUI correctly', () => {
            const component = renderer.create(<NominationResult />);
            expect(component.toJSON()).toMatchSnapshot();
        });
    });

    describe('Component method test', () => {
        it('getCssClass for status method return correct string', () => {
            const wrapper = shallow(<NominationResult />);
            expect(wrapper.instance().getCssClass(1)).toEqual('status-nomination');
            expect(wrapper.instance().getCssClass(2)).toEqual('status-shortlisted');
            expect(wrapper.instance().getCssClass(3)).toEqual('status-winner');
            expect(wrapper.instance().getCssClass(0)).toEqual('');
        });

        it('Component should update when nominations change', () => {
            const wrapper = shallow(<NominationResult />);
            const nextState = { nominations };
            const shouldUpdate = wrapper.instance().shouldComponentUpdate({}, nextState);
            expect(shouldUpdate).toBe(true);
        });

        it('Component should not update when nominations have no change', () => {
            const wrapper = shallow(<NominationResult />);
            wrapper.setState({
                nominations: nominations
            });
            const nextState = { nominations };

            const shouldUpdate = wrapper.instance().shouldComponentUpdate('', nextState);
            expect(shouldUpdate).toBe(false);
        });
    });
});