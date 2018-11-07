import React from "react";
import { shallow, render } from "enzyme";
import renderer from 'react-test-renderer';
import mockAxios from "axios";

jest.mock('axios');
jest.mock('moment', () => () => ({ format: () => '2018–05–11T12:00:00+00:00' }));
jest.mock('../../../resourceHelper');