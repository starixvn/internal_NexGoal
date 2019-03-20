import React from 'react';
import { shallow } from 'enzyme';
import NominationResult from '../NominationResult';
import renderer from 'react-test-renderer';

const nominations = [{
    eoqyId: 1,
    nominationPeriodId: 111,
    period: {
        nominationId: 111,
        title: 'Employee of the year 2018'
    },
    nominationDetails: [{ nominatorId: 363, comment: '' }, { nominatorId: 125, comment: '' }],
    statusId: 1
}, {
    eoqyId: 3,
    nominationPeriodId: 222,
    period: {
        nominationId: 222,
        title: 'Employee of the quarter 3/2018'
    },
    nominationDetails: [{ nominatorId: 125, comment: '' }, { nominatorId: 363, comment: '' }],
    statusId: 2
}];

describe('Test nomination component', () => {
    describe('Component render correctly', () => {
        it('renderUI correctly', () => {
            const component = renderer.create(<NominationResult nominations={nominations} />);
            expect(component.toJSON()).toMatchSnapshot();
        });
    });

    describe('Component method test', () => {
        it('Component should update when nominations change', () => {
            const wrapper = shallow(<NominationResult nominations={[]} />);
            let nextProps = { nominations };
            let shouldUpdate = wrapper.instance().shouldComponentUpdate(nextProps);
            expect(shouldUpdate).toBe(true);
        });

        it('Component should not update when nominations have no change', () => {
            const wrapper = shallow(<NominationResult nominations={nominations} />);
            const nextProps = { nominations };

            const shouldUpdate = wrapper.instance().shouldComponentUpdate(nextProps);
            expect(shouldUpdate).toBe(false);
        });
    });
});